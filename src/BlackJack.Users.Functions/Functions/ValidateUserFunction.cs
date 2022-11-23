using System.Net;
using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using BlackJack.Users.Functions.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BlackJack.Users.Functions.Functions
{
    public class ValidateUserFunction
    {
        private readonly ILogger _logger;
        private readonly TableClient _tableClient;

        public ValidateUserFunction(ILoggerFactory loggerFactory)
        {
            var storageAccountName = Environment.GetEnvironmentVariable("StorageAccountName");
            var credential = new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new EnvironmentCredential(),
                new AzureCliCredential());
            _logger = loggerFactory.CreateLogger<CreateUserFunction>();
            if (storageAccountName != null)
            {
                _tableClient = new TableClient(
                    new Uri(storageAccountName),
                    "users",
                    credential);
            }
        }

        [Function("ValidateUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users/{id}")]
            HttpRequestData req,
            string id
            )
        {
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out var userId))
            {
                try
                {
                    var userEntity = await _tableClient.GetEntityAsync<UserTableEntity>("user", userId.ToString());
                    if (userEntity != null)
                    {
                        return req.CreateResponse(HttpStatusCode.OK);
                    }
                }
                catch (RequestFailedException rfex)
                {
                    _logger.LogError(rfex, "Error retrieving user {userId} from Table Storage", userId);
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving user {userId} from Table Storage", userId);
                }
            }
            return req.CreateResponse(HttpStatusCode.BadRequest);

        }
    }
}
