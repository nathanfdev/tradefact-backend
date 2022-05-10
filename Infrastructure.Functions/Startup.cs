using AzureFunctionsV2.HttpExtensions.Infrastructure;
using Core.Interfaces;
using Core.ServiceBus;
using Infrastructure.Functions;
using Infrastructure.Functions.Helpers;
using Integration.ChargeBee;
using Integration.Segment;
using Integration.TradefactActivityService;
using Mapster;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime;
using System;
using Tradefact.Data;
using Tradefact.Services;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Infrastructure.Functions
{
    public class Startup : FunctionsStartup
    {
        private static string TradefactDBConnectionString = Environment.GetEnvironmentVariable("TradefactDB", EnvironmentVariableTarget.Process);
        private static string envChargebeeSiteName = Environment.GetEnvironmentVariable("ChargebeeSiteName", EnvironmentVariableTarget.Process);
        private static string envChargebeeApiKey = Environment.GetEnvironmentVariable("ChargebeeApiKey", EnvironmentVariableTarget.Process);
        private static string envChargebeeUserAddonSuffix = Environment.GetEnvironmentVariable("ChargebeeUserAddonSuffix", EnvironmentVariableTarget.Process);
        private static string envServiceBusConnection = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
        private static string envSegmentWriteKey = Environment.GetEnvironmentVariable("SegmentWriteKey", EnvironmentVariableTarget.Process);

        public static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public Startup()
        {
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
#if DEBUG
            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom
            //    .Configuration(Configuration)
            //    .WriteTo
            //    .Seq(serverUrl: "http://localhost:5341/")

            //    .CreateLogger();
#endif
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                settings.ContractResolver = new OrderedContractResolver();
#if DEBUG
                settings.Formatting = Formatting.Indented;
#else
                settings.Formatting = Formatting.None;
#endif

                return settings;
            };
        }

        IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
#if DEBUG
            // services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
#endif
            services.AddLogging();
            services.Replace(ServiceDescriptor.Singleton<IHttpExceptionHandler, CustomHttpExceptionHandler>());

            //services.Configure<TransferMateSettings>(configuration.GetSection("TransferMateSettings"));
            //services.AddScoped<IAuthenticator, Authenticator>();
            //services.AddScoped<ISpotRatesApi, SpotRatesApi>();
            //services.AddScoped<AuthenticationApi, AuthenticationApi>();
            //services.AddCosmosClientAndContainerProxy(configuration);
            services.AddSingleton(typeof(IClock), SystemClock.Instance);
            services.AddScoped<IUserResolverService, UserResolverService>();


            //SearchServiceClient serviceClient = CreateSearchServiceClient(configuration);
            //services.AddSingleton(typeof(ISearchServiceClient), serviceClient);

            // SQL Server
            services.AddDbContext<TradefactDbContext>(opts =>
            {
                opts.UseSqlServer(TradefactDBConnectionString, b => b.MigrationsAssembly("Tradefact.Api"));
            });

            services.AddSingleton(typeof(IConfigurationRoot), Configuration);

            // Segment integration
            services.AddSingleton(new SegmentOptions
            {
                Enabled = true,
                WriteKey = envSegmentWriteKey
            });
            services.AddTransient<IAnalyticsService, SegmentService>();

            // Chargebee integration
            services.AddSingleton(new ChargeBeeOptions
            {
                Enabled = true,
                APIKey = envChargebeeApiKey,
                SiteId = envChargebeeSiteName,
                userAddonSuffix = envChargebeeUserAddonSuffix
            });
            services.AddTransient<IBillingService, ChargeBeeService>();

            services.AddSingleton<IServiceBusPersistedConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ServiceBusPersistedConnection>>();

                var serviceBusConnection = new ServiceBusConnectionStringBuilder(envServiceBusConnection);

                return new ServiceBusPersistedConnection(serviceBusConnection, logger);
            });

            services.AddTransient<IServiceBusClient, AzureServiceBusClient>();

            services.AddTransient<ITradefactActivityService, TradefactActivityService>();

            return services;
        }

        public override void Configure(IFunctionsHostBuilder builder) => ConfigureServices(builder.Services, Configuration).BuildServiceProvider(true);
    }
}