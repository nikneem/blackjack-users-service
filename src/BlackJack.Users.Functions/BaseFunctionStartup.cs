using System.Text.Json;
using Azure.Core.Serialization;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackJack.Users.Functions;

public abstract class BaseFunctionStartup<T>
{
    public async Task RunAsync()
    {
        var hostBuilder = CreateHostBuilder();
        var host = hostBuilder.Build();
        await host.RunAsync();
    }

    private IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureHostConfiguration(ConfigureHostConfiguration)
            .ConfigureAppConfiguration(BuildAppConfiguration)
            .ConfigureFunctionsWorkerDefaults(ConfigureFunctionsWorker, ConfigureOptions)
            .ConfigureServices(Configure);

        return hostBuilder;
    }


    private void ConfigureHostConfiguration(IConfigurationBuilder config)
    {
        
    }

    private static void BuildAppConfiguration(HostBuilderContext context, IConfigurationBuilder configBuilder)
    {
        var azureAppConfigurationUrl = Environment.GetEnvironmentVariable("AppConfigEndpoint");
        var credential = new ChainedTokenCredential(new ManagedIdentityCredential(), new EnvironmentCredential(),
            new AzureCliCredential());

        configBuilder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName.ToLower()}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        if (!string.IsNullOrWhiteSpace(azureAppConfigurationUrl))
        {
            try
            {
                configBuilder.AddAzureAppConfiguration(options =>
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

    private void ConfigureFunctionsWorker(HostBuilderContext context, IFunctionsWorkerApplicationBuilder builder)
    {
        //builder.UseNewtonsoftJson(new JsonSerializerSettings
        //{
        //    ContractResolver = new DefaultContractResolver
        //    {
        //        NamingStrategy = new CamelCaseNamingStrategy()
        //    },
        //    NullValueHandling = NullValueHandling.Ignore
        //});
        
        builder.UseDefaultWorkerMiddleware();

    }

    private void ConfigureOptions(WorkerOptions options)
    {

        options.Serializer = new NewtonsoftJsonObjectSerializer(
            new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            });


    }

    /// <summary>
    /// Adds globally used services by all functions and then calls to the Injectioneer to wire
    /// up each of the dependant projects requirements
    /// </summary>
    /// <param name="context">The <see cref="HostBuilderContext"/></param>
    /// <param name="services">The <see cref="IServiceCollection"/> DI container</param>
    private void Configure(HostBuilderContext context, IServiceCollection services)
    {
        services.AddLogging();
        services.AddApplicationInsightsTelemetryWorkerService();
        if (context.Configuration == null) throw new Exception("STARTUP ERROR: Unable to build configuration");
    }
}