using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Flare.Api.Infrastructure
{
    public sealed class SigningUrlHelper
    {
        private readonly byte[] _urlSigningKey;
        private readonly string _portal_url;

        public SigningUrlHelper(string portal_url, byte[] urlSigningKey)
        {
            this._urlSigningKey = urlSigningKey;
            this._portal_url = portal_url;
        }

        public string GenerateSignedURL(string path, string query)
        {
            UriBuilder ub = new UriBuilder(this._portal_url);
            ub.Path = path;
            ub.Query = query;

            var url = new Uri(ub.ToString()).AbsoluteUri;
            using var hmac = new HMACSHA256(this._urlSigningKey);
            var sig = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.ASCII.GetBytes(url)));

            ub.Query = new QueryString(ub.Query).Add("sig", sig).ToString();
            return ub.ToString();
        }
    }
}
