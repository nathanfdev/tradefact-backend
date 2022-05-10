using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Models.TransferMate;
using Microsoft.Extensions.Options;
using Refit;
using Infrastructure.Services.TransferMate;

namespace Infrastruture.Services.TransferMate
{
    public class AuthenticationApi
    {
        private readonly IAuthenticationApi _authenticationApi;

        private readonly IAuthenticator _authenticator;

        private readonly HttpClient _httpClient;

        public AuthenticationApi(IOptions<TransferMateSettings> transferMateSettings, IAuthenticator authenticator)
        {
            _authenticator = authenticator;

            _httpClient = new HttpClient(new AuthenticatedHttpClientHandler(GetToken))
            { BaseAddress = new Uri(transferMateSettings.Value.TransferMateUrl) };

            _authenticationApi = RestService.For<IAuthenticationApi>(_httpClient);
        }

        private async Task<string> GetToken()
        {
            var connectResponse = await _authenticator.Authenticate().ConfigureAwait(false);

            return connectResponse.Token;
        }

        // public async Task<ConnectResponse> Connect([Body] ConnectRequest connectRequest) => throw new NotImplementedException() ;

        public async Task<ConnectionHistoryResponse> ConnectionHistory([Body] ConnectionHistoryRequest connectHistoryRequest) => await _authenticationApi.ConnectionHistory(connectHistoryRequest)
            .ConfigureAwait(false);

        // public async Task Disconnect() => throw new NotImplementedException();
    }
}
