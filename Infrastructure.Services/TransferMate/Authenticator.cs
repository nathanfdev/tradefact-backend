using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Refit;

namespace Infrastructure.Services.TransferMate
{
    public class Authenticator : Core.Models.TransferMate.IAuthenticator
    {
        private readonly Core.Models.TransferMate.IAuthenticationApi _authenticationApi;

        private readonly Core.Models.TransferMate.ConnectRequest _connectRequest;

        private readonly HttpClient _httpClient;

        public Authenticator(IOptions<Core.Models.TransferMate.TransferMateSettings> transferMateSettings)
        {
            _connectRequest = new Core.Models.TransferMate.ConnectRequest
            {
                Username = transferMateSettings.Value.TransferMateUsername,
                Password = transferMateSettings.Value.TransferMatePassword
            };

            //TODO: Implement IHttpClientFactory
            _httpClient = new HttpClient(new HttpClientHandler())
            { BaseAddress = new Uri(transferMateSettings.Value.TransferMateUrl) };

            _authenticationApi = RestService.For<Core.Models.TransferMate.IAuthenticationApi>(_httpClient);
        }

        public async Task<Core.Models.TransferMate.ConnectResponse> Authenticate() => await _authenticationApi.Connect(_connectRequest)
            .ConfigureAwait(false) ;

        public async Task<string> GetToken()
        {
            var response = await _authenticationApi.Connect(_connectRequest).ConfigureAwait(false);

            return response.Token;
        }
    }
}
