using Core.Enums;
using Core.Models;
using FunctionApp.Quotations.Requests;
using FunctionApp.Quotations.Responses;
using Infrastructure.Functions.Helpers;
using Microsoft.Azure.Cosmos;

namespace FunctionApp.Quotations.Functions
{
    public static class QuotationHelpers
    {
        //private readonly IAuthenticationApi _authenticationAPi;
        //private readonly Container _container;
        //private readonly ISpotRatesApi _spotRatesApi;
        //private readonly TransferMateSettings _transferMateSettings;
        //public QuotationFunctions(CosmosClient cosmosClient,
        //                          IOptions<TransferMateSettings> transferMateSettings,
        //                          ISpotRatesApi spotRatesApi,
        //                          IAuthenticationApi authenticationAPi)
        //{
        //_container =
        //    cosmosClient.GetContainer("Importwise", "ImportRates");
        //_transferMateSettings = transferMateSettings.Value;
        //_spotRatesApi = spotRatesApi;
        //_authenticationAPi = authenticationAPi;
        //}
        public static QueryDefinition BuildQuery(ValidatableRequest<QuotationRequest> request)
        {
            var queryOptions = string.Empty;

            if(request.Value.IncoType == IncoTypeEnum.FOB) {

                // TODO: determine what impact that has on what results to return
                queryOptions = string.Empty;
            }
            if(request.Value.LoadType == LoadTypeEnum.FCL) {
                queryOptions += string.Empty;
            } else {
                queryOptions += string.Empty;
            }
            if(request.Value.Load != null) {
                if(request.Value.Load.FT20 > 0) {
                    queryOptions += $" AND IS_DEFINED(IR.{LoadEnum.ft20}) AND NOT IS_NULL(IR.{LoadEnum.ft20})";
                }
                if(request.Value.Load.FT40 > 0) {
                    queryOptions += $" AND IS_DEFINED(IR.{LoadEnum.ft40}) AND NOT IS_NULL(IR.{LoadEnum.ft40})";
                }
                if(request.Value.Load.FTHHQ40 > 0) {
                    queryOptions +=
                        $" AND IS_DEFINED(IR.{LoadEnum.ftHHQ40}) AND NOT IS_NULL(IR.{LoadEnum.ftHHQ40})";
                }
                if(request.Value.Load.FTHQ45 > 0) {
                    queryOptions += $" AND IS_DEFINED(IR.{LoadEnum.ftHQ45}) AND NOT IS_NULL(IR.{LoadEnum.ftHQ45})";
                }
            }

            // We have a few options which are nullable, so a Linq Query is probably not the most suitable.
            // Thoughts: Capitalize values, which are "Codes"
            return new QueryDefinition($"SELECT * FROM ImportRates IR WHERE IR.portOfLoading = @portOfLoading AND IR.portOfDischarge = @portOfDischarge {queryOptions}")
                .WithParameter("@portOfLoading", request.Value.PortOfLoading)
                .WithParameter("@portOfDischarge", request.Value.PortOfDischarge);

            //.WithParameter("@partnerCode", quotationRequest.PartnerCode)
            //.WithParameter("@partnerCode", quotationRequest.PartnerCode)
            //.WithParameter("@placeOfDispatch", request.Value.PlaceOfDispatch);

            //FeedIterator<PartnerRate> resultSetIterator = _container.GetItemQueryIterator<PartnerRate>(query);

            //while(resultSetIterator.HasMoreResults)
            //{
            //    partnerRates.AddRange(await resultSetIterator.ReadNextAsync().ConfigureAwait(false));
            //}
        }

        public static decimal CalculateCustomsValue(PartnerRate item,
                                                    QuotationResponse quotationResponse,
                                                    decimal totalCost)
        {
            if(item.Customs.HasValue) {
                var customsCost = item.Customs.Value;

                if(customsCost > 0) {
                    totalCost += customsCost;
                    var lineItem = new LineItemModel { Description = "Customs", Quantity = 1, Amount = customsCost };

                    quotationResponse.LineItems.Add(lineItem);
                }
            }

            return totalCost;
        }

        public static decimal CalculateDocumentFees(PartnerRate item,
                                                    QuotationResponse quotationResponse,
                                                    decimal totalCost)
        {
            if(item.Documents.HasValue) {
                var documentFees = item.Documents.Value;
                totalCost += documentFees;
                var lineItem = new LineItemModel { Description = "Document Fees", Quantity = 1, Amount = documentFees };

                quotationResponse.LineItems.Add(lineItem);
            }

            return totalCost;
        }

        public static decimal CalculateInsuranceValue(ValidatableRequest<QuotationRequest> request,
                                                      PartnerRate item,
                                                      QuotationResponse quotationResponse,
                                                      decimal totalCost)
        {
            if(request.Value.InsuranceValue.HasValue && item.Insurance.HasValue) {
                var insuranceCost = request.Value.InsuranceValue.Value * item.Insurance.Value;

                if(insuranceCost > 0) {
                    totalCost += insuranceCost;
                    var lineItem = new LineItemModel { Description = "Insurance", Quantity = 1, Amount = insuranceCost };

                    quotationResponse.LineItems.Add(lineItem);
                }
            }

            return totalCost;
        }

        public static decimal CalculatePortFees(PartnerRate item,
                                                QuotationResponse quotationResponse,
                                                decimal totalCost)
        {
            if(item.PortFees.HasValue) {
                var portFees = item.PortFees.Value;
                totalCost += portFees;
                var lineItem = new LineItemModel { Description = "Port Fees", Quantity = 1, Amount = portFees };

                quotationResponse.LineItems.Add(lineItem);
            }

            return totalCost;
        }

        public static decimal CalculateSeaFreight(ValidatableRequest<QuotationRequest> request,
                                                  PartnerRate item,
                                                  QuotationResponse quotationResponse,
                                                  decimal totalCost)
        {
            if(request.Value.Load != null) {
                decimal seaFreight = 0;

                if((request.Value.Load.FT20 > 0) && item.FT20.HasValue) {
                    seaFreight += request.Value.Load.FT20 * item.FT20.Value;
                }
                if((request.Value.Load.FT40 > 0) && item.FT40.HasValue) {
                    seaFreight += request.Value.Load.FT40 * item.FT40.Value;
                }
                if((request.Value.Load.FTHHQ40 > 0) && item.FTHHQ40.HasValue) {
                    seaFreight += request.Value.Load.FTHHQ40 * item.FTHHQ40.Value;
                }
                if((request.Value.Load.FTHQ45 > 0) && item.FTHQ45.HasValue) {
                    seaFreight += request.Value.Load.FTHQ45 * item.FTHQ45.Value;
                }
                var lineItem = new LineItemModel { Description = "Sea Freight", Quantity = 1, Amount = seaFreight };

                quotationResponse.LineItems.Add(lineItem);

                totalCost += seaFreight;
            }

            return totalCost;
        }

        public static decimal CalculateThcValues(PartnerRate item,
                                                 QuotationResponse quotationResponse,
                                                 decimal totalCost)
        {
            if(item.InboundThc.HasValue) {
                var inboundThcFees = item.InboundThc.Value;
                totalCost += inboundThcFees;
                var lineItem = new LineItemModel
                { Description = "Inbound THC Fees", Quantity = 1, Amount = inboundThcFees };

                quotationResponse.LineItems.Add(lineItem);
            }

            return totalCost;
        }

        // The TransferMate Payments service enables you to make payments to third parties.
        // Using the Payments API, you make a payment using the following three steps, the first of which is optional:
        // Get a Currency Exchange Rate for Review(Optional)
        // You can query the exchange rate for a currency. 
        // The returned rate remains valid for 90 seconds.The Payments service does not create a record in the TransferMate system in this case. 
        // If you are happy with the retrieved rate, you can use it in the lock-in and authorization steps that follow.
        // The returned rate remains valid for 90 seconds between the query time and authorization(see step 3).
        // Lock in an Exchange Rate for a Payment Transaction
        // This operation is similar to the action in the previous step.
        // In this case however, the Payments service creates a record in the TransferMate system.
        // The returned rate remains valid for 90 seconds between the lock-in time and authorization(see the next step).
        // Authorize One or More Payment Transactions
        // You can authorize one or more payments in a batch transaction.
        // If the batch transaction includes payment transactions that are not recognized by the service, these are highlighted as erroneous payment transactions.
        // To verify with Barry, and team
        // The logical way to approach this is to get Spot Rates, use the mid market value, for the quotation estimate.
        // Subsequently, when the company chooses a quotation, get the currency exchange rate for review (bank to bank), valid 90 seconds until.
        // This will include transaction fees.
        // If agreed, then continue by locking in the rate for the transaction, valid 90 seconds.
        // And finally, Authorise it, to complete.
        // This mitigates socket exhaustion via the HTTPClient calls on the Host app service.
        //public static async Task<List<QuotationResponse>> EstimateQuotationCurrencyConversion(ILogger logger,
        //                                                                                       List<QuotationResponse> quotationResponses,
        //                                                                                       HashSet<string> currenciesToConvert,
        //                                                                                       string companyCurrency)
        //{
        //    try
        //    {
        //        var getRatesrequest = new GetRatesrequest { Amount = 1 };
        //        foreach(var currencyToConvert in currenciesToConvert)
        //        {
        //            var pair = new Pair { Dst = companyCurrency, Src = currencyToConvert };
        //            getRatesrequest.Pairs.Add(pair);
        //        }
        //        var spotRates = await _spotRatesApi.GetSpotRates(getRatesrequest).ConfigureAwait(false);
        //        foreach(var quotationResponse in quotationResponses)
        //        {
        //            var spotRate = spotRates.Find(s => s.Currency == quotationResponse.Currency + companyCurrency);
        //            var midRate = spotRate.Mid;
        //            var midRateDecimal = decimal.Parse(midRate);
        //            // Loop through each quotation response, 
        //            quotationResponse.TotalCostConverted = quotationResponse.TotalCost * midRateDecimal;
        //            quotationResponse.CurrencyConverted = companyCurrency;
        //            // Possibly each line item ?                       
        //        }
        //    } catch(Exception ex)
        //    {
        //        logger.LogError("Error connecting to TransferMate", ex);
        //    }
        //    return quotationResponses;
        //}
    }
}