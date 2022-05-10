using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
using FunctionApp.Quotations.Requests;
using FunctionApp.Quotations.Responses;
using Infrastructure.Functions.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using NSwag.Annotations.AzureFunctionsV2;

namespace FunctionApp.Quotations.Functions
{
    public class QuotationFunctions
    {
        private readonly ICosmosContainerProxy _proxy;

        private readonly ICosmosRepository _repository;

        public QuotationFunctions(ICosmosRepository repository, ICosmosContainerProxy proxy)
        {
            _repository = repository;
            _proxy = proxy;
        }

        [SwaggerResponse(200, typeof(QuotationResponse), Description = "OK result")]
        [SwaggerRequestBodyType(typeof(QuotationRequest))]
        [FunctionName(nameof(GetQuotations))]
        public async Task<IActionResult> GetQuotations([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "")]
            HttpRequest req, ILogger logger)
        {
            var request =
                await req.GetJsonBody<QuotationRequest, QuotationRequestValidator>(logger).ConfigureAwait(false);

            if(!request.IsValid) {
                return request.ToBadRequest();
            }

            var quotationResponses = new List<QuotationResponse>();
            var currenciesToConvert = new HashSet<string>();

            //var companyCurrency = "USD";
            // TODO: Retrieve company currency
            // Query CosmosDB for Quotations                
            var query = QuotationHelpers.BuildQuery(request);

            var partnerQuotations = await _repository.GetAsync<PartnerRate>(query, _proxy.ImportRates)
                .ConfigureAwait(false);

            // Create quotationResponses 
            foreach(var partnerQuotation in partnerQuotations) {
                var quotationResponse = new QuotationResponse();

                var quoteCost = 0m;

                quoteCost = QuotationHelpers.CalculateSeaFreight(
                                request,
                                partnerQuotation,
                                quotationResponse,
                                quoteCost);

                quoteCost = QuotationHelpers.CalculateCustomsValue(partnerQuotation, quotationResponse, quoteCost);

                quoteCost = QuotationHelpers.CalculateInsuranceValue(
                                request,
                                partnerQuotation,
                                quotationResponse,
                                quoteCost);

                quoteCost = QuotationHelpers.CalculatePortFees(partnerQuotation, quotationResponse, quoteCost);

                quoteCost = QuotationHelpers.CalculateDocumentFees(partnerQuotation, quotationResponse, quoteCost);

                quoteCost = QuotationHelpers.CalculateThcValues(partnerQuotation, quotationResponse, quoteCost);

                quotationResponse.TotalCost = quoteCost;

                quotationResponse.IsoCurrency = "GBP";

                //TODO: lookup from partner.
                currenciesToConvert.Add(quotationResponse.IsoCurrency);

                quotationResponse.TransitTime = partnerQuotation.TransitTime;
                quotationResponses.Add(quotationResponse);
            }

            //var convertedQuotationResponses = await EstimateQuotationCurrencyConversion(logger,
            //                                                                            quotationResponses,
            //                                                                            currenciesToConvert,
            //                                                                            companyCurrency);

            return new OkObjectResult(quotationResponses);
        }
    }
}