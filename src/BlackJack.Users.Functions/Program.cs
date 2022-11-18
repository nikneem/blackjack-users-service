using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(c =>
    {
        c.AddEnvironmentVariables();
        var userAssignedClientId = Environment.GetEnvironmentVariable("UserAssignedIdentityClientId");
        var identity = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });
        var azureAppConfigurationUrl = Environment.GetEnvironmentVariable("AppConfigEndpoint");
        if (!string.IsNullOrWhiteSpace(azureAppConfigurationUrl))
        {
            try
            {
                c.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(azureAppConfigurationUrl), identity)
                        .ConfigureKeyVault(kv => { kv.SetCredential(identity); });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to configure service using Azure App Configuration service", ex);
            }
        }
    })
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.Services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

host.Run();
