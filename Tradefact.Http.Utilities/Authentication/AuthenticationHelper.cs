using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tradefact.Http.Utilities.Authentication
{
    public static class AuthenticationHelper
    {
        private static string _token = null;

        public async static Task<string> GetToken(string baseUrl)
        {
            if (!string.IsNullOrEmpty(_token))
            {
                return _token;
            }
            var response = await HttpRequestFactory.PostAnonymous(baseUrl, "authentication/token", new { Username = "marc" });
            var content = response.ContentAsType<TokenResponse>();
            _token = content.Token;
            return _token;
        }
    }
}
