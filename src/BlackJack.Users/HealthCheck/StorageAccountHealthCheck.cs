using Azure.Data.Tables;
using BlackJack.Core.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace BlackJack.Users.HealthCheck;

    public class StorageAccountHealthCheck : IHealthCheck
    {
        private const string TableName = "users";
        private readonly IOptions<AzureConfiguration> _configOptions;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            var config = _configOptions.Value;
            try
            {
                var storageUri = new Uri($"https://{config.StorageAccount}.table.core.windows.net");
                var tableClient = new TableClient(
                    storageUri,
                    TableName,
                    new TableSharedKeyCredential(config.StorageAccount, config.StorageKey));
                await tableClient.CreateIfNotExistsAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        public StorageAccountHealthCheck(IOptions<AzureConfiguration> configOptions)
        {
            _configOptions = configOptions;
        }
    }
