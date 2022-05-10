using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Functions.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Integration.External.ProductCatalog
{
    public static partial class ProductCatalagFunctions
    {
        [SwaggerResponse(200, typeof(List<ProductResponse>), Description = "Created result")]
        [FunctionName(nameof(GetProducts))]
        public static async Task<IActionResult> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req, ILogger logger, ClaimsPrincipal principal)
        {
            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
                Console.WriteLine(requestBody);
            }

            List<Product> products = Enumerable.Range(1, 10).Select(x => new Product { SKU = $"ABC-DEF-{x.ToString().PadLeft(4, '0')}"  }).ToList();
            return new OkObjectResult(products.Adapt<List<ProductResponse>>());
        }

        [SwaggerResponse(200, typeof(ProductResponse), Description = "Created result")]
        [SwaggerRequestBodyType(typeof(CreateProductRequest))]
        [FunctionName(nameof(CreateProduct))]
        public static async Task<IActionResult> CreateProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger, ClaimsPrincipal principal)
        {
            var request =
                await req.GetJsonBody<CreateProductRequest, CreateProductRequestValidator>(logger).ConfigureAwait(false);

            if (!request.IsValid)
            {
                return request.ToBadRequest();
            }

            var item = request.Value.Adapt<Product>();
            return new OkObjectResult(item.Adapt<ProductResponse>());
        }

        [FunctionName(nameof(Post))]
        public static async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req, ILogger logger)
        {
            string requestBody = String.Empty;
            using (StreamReader streamReader =  new  StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
                Console.WriteLine(requestBody);
            }

            // Return a 200 OK to the client
            return new OkResult();
        }        
    }
}
