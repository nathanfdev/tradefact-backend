//using Core.Interfaces;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infrastructure.Services
//{
//    public class UserService : IUserService
//    {
//        private const string GraphBaseUrl = "https://graph.windows.net/";
//        private const string GraphVersionQueryString = "?" + GraphVersion;
//        private const string GraphVersion = "api-version=1.6";

//        private readonly AuthenticationContext _authContext;
//        private readonly ClientCredential _clientCreds;
//        private readonly string _graphUrl;

//        public UserService(ISettingService settingService) : this(settingService.GetGraphTenantId(),
//                                                                  settingService.GetGraphClientId(),
//                                                                  settingService.GetGraphClientSecret())
//        { }

//        public UserService(string tenantId, string clientId, string clientSecret)
//        {
//            _graphUrl = $"{GraphBaseUrl}{tenantId}";

//            var authority = $"https://login.microsoftonline.com/{tenantId}";
//            _authContext = new AuthenticationContext(authority);
//            _clientCreds = new ClientCredential(clientId, clientSecret);
//        }

//        public async Task<(Core.Models.User, string error)> CreateUser(CreateUser newUser)
//        {
//            var url = $"{_graphUrl}/users{GraphVersionQueryString}";

//            var client = new HttpClient();
//            client.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", await GetAccessToken().ConfigureAwait(false));

//            var payload = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");
//            var response = await client.PostAsync(url, payload).ConfigureAwait(false);
//            if(response.IsSuccessStatusCode)
//            {
//                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                var user = JsonConvert.DeserializeObject<Core.Models.User>(json);
//                return (user, null);
//            }

//            if(response.StatusCode == HttpStatusCode.BadRequest)
//            {
//                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                var badRequest = JsonConvert.DeserializeObject<BadRequestResponse>(json);
//                return (null, badRequest.ErrorMessage);
//            }

//            return (null, $"Error Creating User. HTTP Status Code: {(int)response.StatusCode}");
//        }

//        public async Task<(IEnumerable<Core.Models.User>, string error)> GetUsers()
//        {
//            var url = $"{_graphUrl}/users{GraphVersionQueryString}";

//            var client = new HttpClient();
//            client.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", await GetAccessToken().ConfigureAwait(false));

//            var response = await client.GetAsync(url).ConfigureAwait(false);
//            if(response.IsSuccessStatusCode)
//            {
//                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                var result = JsonConvert.DeserializeObject<UsersResult>(json);
//                return (result.Value, null);
//            }

//            if(response.StatusCode == HttpStatusCode.BadRequest)
//            {
//                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                var badRequest = JsonConvert.DeserializeObject<BadRequestResponse>(json);
//                return (null, badRequest.ErrorMessage);
//            }

//            return (null, $"Error Getting Users. HTTP Status Code: {(int)response.StatusCode}");
//        }

//        public async Task<(Core.Models.User, string error)> GetUserById(string userId)
//        {
//            if(string.IsNullOrWhiteSpace(userId))
//                throw new ArgumentNullException(nameof(userId));

//            var url = $"{_graphUrl}/users/{userId}{GraphVersionQueryString}";

//            var client = new HttpClient();
//            client.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", await GetAccessToken().ConfigureAwait(false));

//            var response = await client.GetAsync(url).ConfigureAwait(false);
//            if(response.IsSuccessStatusCode)
//            {
//                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                var result = JsonConvert.DeserializeObject<Core.Models.User>(json);
//                return (result, null);
//            }

//            if(response.StatusCode == HttpStatusCode.BadRequest)
//            {
//                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                var badRequest = JsonConvert.DeserializeObject<BadRequestResponse>(json);
//                return (null, badRequest.ErrorMessage);
//            }

//            return (null, $"Error Getting User. HTTP Status Code: {(int)response.StatusCode}");
//        }

//        private async Task<string> GetAccessToken()
//        {
//            var authResult = await _authContext.AcquireTokenAsync(GraphBaseUrl, _clientCreds).ConfigureAwait(false);
//            return authResult.AccessToken;
//        }
//    }
//}