using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Principal;

var host = new HostBuilder()
    .ConfigureAppConfiguration(c =>
    {
        c.AddEnvironmentVariables();
        try
        {
            var azureAppConfigurationUrl = Environment.GetEnvironmentVariable("AppConfigEndpoint");
            var credential = new ChainedTokenCredential(new ManagedIdentityCredential(), new EnvironmentCredential(),
                new AzureCliCredential());

            if (!string.IsNullOrWhiteSpace(azureAppConfigurationUrl))
            {
                try
                {
                    c.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(new Uri(azureAppConfigurationUrl), credential)
                            .ConfigureKeyVault(kv => { kv.SetCredential(credential); });
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to configure service using Azure App Configuration service", ex);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }


    })
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.Services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

host.Run();
