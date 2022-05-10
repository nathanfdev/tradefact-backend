using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Refit;

namespace Infrastructure.Services.TransferMate
{
    public class TransferMateApi : Core.Models.TransferMate.ITransferMateApi
    {
        private readonly Core.Models.TransferMate.IAuthenticator _authenticator;

        private readonly HttpClient _httpClient;

        private readonly Core.Models.TransferMate.ITransferMateApi _transferMateApi;

        public TransferMateApi(IOptions<Core.Models.TransferMate.TransferMateSettings> transferMateSettings,
                               Core.Models.TransferMate.IAuthenticator authenticator)
        {
            _authenticator = authenticator;

            _httpClient = new HttpClient(new AuthenticatedHttpClientHandler(GetToken))
            { BaseAddress = new Uri(transferMateSettings.Value.TransferMateUrl) };

            _transferMateApi = RestService.For<Core.Models.TransferMate.ITransferMateApi>(_httpClient);
        }

        private async Task<string> GetToken()
        {
            var connectResponse = await _authenticator.Authenticate().ConfigureAwait(false);

            return connectResponse.Token;
        }

        public async Task<Core.Models.TransferMate.AddBankResponse> AddBank([Body] Core.Models.TransferMate.AddBankRequest addBankRequest) => await _transferMateApi.AddBank(addBankRequest)
            .ConfigureAwait(false) ;

        public Task<Core.Models.TransferMate.ConnectResponse> Connect([Body] Core.Models.TransferMate.ConnectRequest connectRequest) =>
            //TODO: This feels weird
 throw new NotImplementedException() ;

        public async Task<Core.Models.TransferMate.ConnectionHistoryResponse> ConnectionHistory([Body] Core.Models.TransferMate.ConnectionHistoryRequest connectHistoryRequest) => await _transferMateApi.ConnectionHistory(connectHistoryRequest)
            .ConfigureAwait(false) ;

        public async Task Disconnect() => await _transferMateApi.Disconnect().ConfigureAwait(false) ;

        public async Task<Core.Models.TransferMate.EditBankResponse> EditBank([Body] Core.Models.TransferMate.EditBankRequest editBankRequest) => await _transferMateApi.EditBank(editBankRequest)
            .ConfigureAwait(false) ;

        public async Task<List<Core.Models.TransferMate.GetRatesResponse>> GetSpotRates([Body] Core.Models.TransferMate.GetRatesrequest getRatesrequest) => await _transferMateApi.GetSpotRates(getRatesrequest)
            .ConfigureAwait(false) ;

        public async Task<Core.Models.TransferMate.VerifyBankResponse> VerifyBank([Body] Core.Models.TransferMate.VerifyBankRequest verifyBankRequest) => await _transferMateApi.VerifyBank(verifyBankRequest)
            .ConfigureAwait(false) ;

        public async Task<Core.Models.TransferMate.ViewBanksResponse> ViewBanks([Body] Core.Models.TransferMate.ViewBanksRequest viewBanksRequest) => await _transferMateApi.ViewBanks(viewBanksRequest)
            .ConfigureAwait(false) ;
    }
}
