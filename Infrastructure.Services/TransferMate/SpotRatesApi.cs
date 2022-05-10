using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Refit;

namespace Infrastructure.Services.TransferMate
{
    public class SpotRatesApi : Core.Models.TransferMate.ISpotRatesApi
    {
        private readonly Core.Models.TransferMate.IAuthenticator _authenticator;

        private readonly HttpClient _httpClient;

        //TODO: Refactor the API declarations and interfaces to a more logical structure.. ?
        private readonly Core.Models.TransferMate.ISpotRatesApi _spotRatesApi;

        public SpotRatesApi(IOptions<Core.Models.TransferMate.TransferMateSettings> transferMateSettings,
                            Core.Models.TransferMate.IAuthenticator authenticator)
        {
            _authenticator = authenticator;

            _httpClient = new HttpClient(new AuthenticatedHttpClientHandler(GetToken))
            { BaseAddress = new Uri(transferMateSettings.Value.TransferMateUrl) };

            _spotRatesApi = RestService.For<Core.Models.TransferMate.ISpotRatesApi>(_httpClient);
        }

        private async Task<string> GetToken()
        {
            var connectResponse = await _authenticator.Authenticate().ConfigureAwait(false);

            return connectResponse.Token;
        }

        public async Task<List<Core.Models.TransferMate.GetRatesResponse>> GetSpotRates([Body] Core.Models.TransferMate.GetRatesrequest getRatesrequest) => await _spotRatesApi.GetSpotRates(getRatesrequest)
            .ConfigureAwait(false) ;
    }
}
