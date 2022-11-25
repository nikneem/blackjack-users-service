using System.Net;
using System.Text.Json;
using Azure.Core.Serialization;
using Azure.Data.Tables;
using Azure.Identity;
using BlackJack.Users.Functions.DataTransferObjects;
using BlackJack.Users.Functions.Entities;
using BlackJack.Users.Functions.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlackJack.Users.Functions.Functions
{
    public class CreateUserFunction
    {
        private readonly ILogger _logger;
        private readonly TableClient _tableClient;

        public CreateUserFunction(ILoggerFactory loggerFactory)
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

        [Function("CreateUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users")]
            HttpRequestData req
        )
        {
            var dto = new UserDto
            {
                UserId = Guid.NewGuid()
            };
            _logger.LogInformation("Generated unique GUID for user {userId}", dto.UserId);

            try
            {
                _logger.LogInformation("Storing User ID {userId} in Table Storage", dto.UserId);
                var entity = new UserTableEntity
                {
                    PartitionKey = "user",
                    RowKey = dto.UserId.ToString(),
                    Timestamp = DateTimeOffset.UtcNow
                };
                await _tableClient.AddEntityAsync(entity);
                _logger.LogInformation("Storing User ID {userId} in Table Storage...OK", dto.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to store new user in data store");

                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                errorResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var errorMessage = new ErrorMessageDto
                {
                    ErrorCode = "FailedToCreateUser",
                    TranslationKey = "Users.FailedToCreateUser",
                    ErrorMessage = ex.Message
                };
                await errorResponse.WriteStringAsync(errorMessage.Serialize());
                return errorResponse;
            }
            

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(dto.Serialize());
            return response;

        }
    }
}
