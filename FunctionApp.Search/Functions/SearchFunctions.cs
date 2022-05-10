using Core.Interfaces;
using FunctionApp.Search.Requests;
using Infrastructure.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;
using System;
using System.Threading.Tasks;

namespace FunctionApp.Search.Functions
{
    public class SearchFunctions
    {
        readonly string _indexPrefix;
        readonly ICosmosContainerProxy _proxy;

        readonly ICosmosRepository _repository;

        readonly ISearchServiceClient _searchServiceClient;

        public SearchFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy, ISearchServiceClient searchServiceClient, IConfigurationRoot configuration)
        {
            //SearchServiceClient serviceClient = CreateSearchServiceClient(configuration);

            _indexPrefix = configuration["SearchIndexPrefix"];

            _repository = repository;
            _proxy = proxy;
            _searchServiceClient = searchServiceClient;

            //TODO: The code below should only be executed, if the models have changed, or to reset the indexes.

            //var indexName = $"{_indexPrefix}-company-index";
            //DeleteIndexIfExists(indexName);
            //CreateIndex<SearchableCompany>(indexName);
            //CreateDataSourceAndIndexer(indexName, "Companies");

            //indexName = $"{_indexPrefix}-imports-index";

            //DeleteIndexIfExists(indexName);
            //CreateIndex<SearchableImport>(indexName);
            //CreateDataSourceAndIndexer(indexName, "Imports");
        }

        void CreateDataSourceAndIndexer(string indexName, string collection)
        {
            DataSource dataSource = DataSource.CosmosDb(indexName,
                                                        "AccountEndpoint=https://dev-tradefact-cosmosdb.documents.azure.com:443/;AccountKey=pIwTnb2dJHDfstvyB3qL4pyj40n4Ovzm7h6nRNfrciwi1z5nE3SGDbe8A2ugFJ6nThcrxel9WiTrBVX3SnedZg==;Database=TradeFact;",
                                                        collection);

            _searchServiceClient.DataSources.CreateOrUpdate(dataSource);

            var indexer = new Indexer(name: $"{indexName}er", dataSourceName: dataSource.Name, targetIndexName: indexName, schedule: new IndexingSchedule(TimeSpan.FromHours(1)));

            bool exists = _searchServiceClient.Indexers.Exists(indexer.Name);
            if(exists)
            {
                _searchServiceClient.Indexers.Reset(indexer.Name);
                _searchServiceClient.Indexers.Run(indexer.Name);
            }

            _searchServiceClient.Indexers.CreateOrUpdate(indexer);
        }

        void CreateIndex<T>(string indexName)
        {
            var definition = new Microsoft.Azure.Search.Models.Index { Name = indexName, Fields = FieldBuilder.BuildForType<T>() };
            _searchServiceClient.Indexes.Create(definition);
        }

        static SearchServiceClient CreateSearchServiceClient(IConfiguration configuration)
        {
            string searchServiceName = configuration["SearchServiceName"];
            string adminApiKey = configuration["SearchServiceAdminApiKey"];

            return new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
        }


        void DeleteIndexIfExists(string indexName)
        {
            if(_searchServiceClient.Indexes.Exists(indexName))
            {
                _searchServiceClient.Indexes.Delete(indexName);
            }
        }

        //[SwaggerResponse(200, typeof(Company[]), Description = "OK result")]
        //[SwaggerRequestBodyType(typeof(CompanySearchRequest))]
        //[FunctionName(nameof(SearchCompanies))]
        //public async Task<IActionResult> SearchCompanies([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        //    HttpRequest req, ILogger logger)
        //{
        //    var request = await req.GetJsonBody<CompanySearchRequest, CompanySearchRequestValidator>(logger).ConfigureAwait(false);

        //    if(!request.IsValid)
        //    {
        //        return request.ToBadRequest();
        //    }

        //    var searchTerm = request.Value.SearchTerm.ToLowerInvariant();

        //    var searchQuery = new QueryDefinition($"SELECT * FROM c WHERE CONTAINS (LOWER(c.name), @searchTerm) AND c.type=\"Company\"")
        //                            .WithParameter("@searchTerm", searchTerm);

        //    var item = await _repository.GetAsync<Company>(searchQuery, _proxy.Companies).ConfigureAwait(false);

        //    return new OkObjectResult(item);
        //}

        [SwaggerResponse(200, typeof(SearchableCompany[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(CompanySearchRequest))]
        [FunctionName(nameof(SearchCompanies))]
        public async Task<IActionResult> SearchCompanies([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var indexName = $"{_indexPrefix}-company-index";

            SearchParameters parameters;

            ISearchIndexClient indexClient = _searchServiceClient.Indexes.GetClient(indexName);

            var request =
                await req.GetJsonBody<CompanySearchRequest, CompanySearchRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var searchRequest = request.Value;

            parameters = new SearchParameters
            {
                Select = SearchableCompany.SearchableFields.Split(","),
                IncludeTotalResultCount = true,
                Top = searchRequest.PageSize,
                Skip = searchRequest.PageSize * (searchRequest.PageNumber - 1),
                QueryType = QueryType.Full,
                SearchMode = SearchMode.Any,
                Filter = $"type eq 'Company'  { searchRequest.Filter }"
            };

            DocumentSearchResult<SearchableCompany> results;
            try
            {
                results = indexClient.Documents.Search<SearchableCompany>(searchRequest.SearchTerm, parameters);
            } catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

            return new OkObjectResult(results);
        }

        //[SwaggerResponse(200, typeof(SearchableProduct[]), Description = "OK result")]
        //[SwaggerRequestBodyType(typeof(ProductSearchRequest))]
        //[FunctionName(nameof(SearchProducts))]
        //public async Task<IActionResult> SearchProducts([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        //    HttpRequest req, ILogger logger)
        //{
        //    var indexName = $"{_indexPrefix}-company-index";

        //    SearchParameters parameters;

        //    ISearchIndexClient indexClient = _searchServiceClient.Indexes.GetClient(indexName);

        //    var request =
        //        await req.GetJsonBody<ProductSearchRequest, ProductSearchRequestValidator>(logger).ConfigureAwait(false);

        //    if (!request.IsValid)
        //    {
        //        return request.ToBadRequest();
        //    }

        //    var searchRequest = request.Value;

        //    parameters = new SearchParameters
        //    {
        //        Select = SearchableProduct.SearchableFields.Split(","),
        //        IncludeTotalResultCount = true,
        //        Top = searchRequest.PageSize,
        //        Skip = searchRequest.PageSize * (searchRequest.PageNumber - 1),
        //        QueryType = QueryType.Full,
        //        SearchMode = SearchMode.Any,
        //        Filter = $"type eq 'Product'  { searchRequest.Filter }"
        //    };

        //    DocumentSearchResult<SearchableCompany> results;
        //    try
        //    {
        //        results = indexClient.Documents.Search<SearchableProduct>(searchRequest.SearchTerm, parameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestObjectResult(ex);
        //    }

        //    return new OkObjectResult(results);
        //}

        [SwaggerResponse(200, typeof(SearchableImport[]), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(SearchableImport))]
        [FunctionName(nameof(SearchImports))]
        public async Task<IActionResult> SearchImports([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            var indexName = $"{_indexPrefix}-imports-index";

            SearchParameters parameters;

            ISearchIndexClient indexClient = _searchServiceClient.Indexes.GetClient(indexName);

            var request =
                await req.GetJsonBody<ImportSearchRequest, ImportSearchRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var searchRequest = request.Value;

            parameters = new SearchParameters
            {
                Select = SearchableImport.SearchableFields.Split(","),
                IncludeTotalResultCount = true,
                Top = searchRequest.PageSize,
                Skip = searchRequest.PageSize * (searchRequest.PageNumber - 1),
                QueryType = QueryType.Full,
                SearchMode = SearchMode.Any,
                Filter = $"type eq 'Import'  { searchRequest.Filter }"
            };

            DocumentSearchResult<SearchableImport> results;
            try
            {
                results = indexClient.Documents.Search<SearchableImport>(searchRequest.SearchTerm, parameters);
            } catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

            return new OkObjectResult(results);
        }
    }
}