using System;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Functions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string CosmosDbAuthKey = "CosmosDBAuthKey";

        private const string CosmosDbEndPoint = "CosmosDBEndPoint";

        public static IServiceCollection AddCosmosClientAndContainerProxy(this IServiceCollection services,
                                                                          IConfiguration configuration)
        {
            var endpoint = configuration[CosmosDbEndPoint];
            if(string.IsNullOrEmpty(endpoint)) {
                throw new ArgumentNullException("Please specify a valid endpoint in the appSettings.json file or your Azure Functions Settings.");
            }
            var authKey = configuration[CosmosDbAuthKey];
            if(string.IsNullOrEmpty(authKey)) {
                throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json file or your Azure Functions Settings.");
            }

            var client = new CosmosClientBuilder(endpoint, authKey)
            .WithApplicationName("Importwise")
                .WithConnectionModeDirect()
                .WithSerializerOptions(new CosmosSerializationOptions
                { IgnoreNullValues = true, PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .WithApplicationRegion(Regions.WestEurope)
                .WithConsistencyLevel(ConsistencyLevel.Session)
                .WithThrottlingRetryOptions(TimeSpan.FromSeconds(10), 5)
                .Build();

            services.AddSingleton(client);
            services.AddSingleton(typeof(ICosmosRepository), typeof(CosmosRepository));
            services.AddSingleton<ICosmosContainerProxy>(new CosmosContainerProxy(client));

            return services;
        }

        //public static IServiceCollection AddCosmosGenericRepository(this IServiceCollection services,
        //                                                            IConfiguration configuration) => services.AddSingleton<ICosmosContainerProvider, CosmosContainerProvider>()
        //    .AddSingleton(typeof(ICosmosRepository), typeof(CosmosRepository));
        //.Configure<RepositoryOptions>(configuration.GetSection(nameof(RepositoryOptions)));
    }
}