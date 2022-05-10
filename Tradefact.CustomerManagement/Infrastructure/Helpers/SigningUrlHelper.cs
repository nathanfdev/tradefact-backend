using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tradefact.Portal.Infrastructure.Helpers
{
    public class SigningUrlHelper
    {
        private readonly byte[] _urlSigningKey;

        public SigningUrlHelper(byte[] urlSigningKey)
        {
            this._urlSigningKey = urlSigningKey;
        }

        public string GetUrlWithoutSignature(HttpRequest request)
        {
            var restOfQuery = QueryString.Create(request.Query.Where(x => x.Key != "sig"));

            var url = request.GetEncodedUrl();
            var ub = new UriBuilder(url);
            ub.Query = restOfQuery.ToString();
            return ub.Uri.AbsoluteUri;
        }

        public bool SignatureIsValid(string candidate, HttpRequest request)
        {
            var sig = request.Query["sig"];
            var receivedSignature = Convert.FromBase64String(sig.ToString());

            using var hmac = new HMACSHA256(this._urlSigningKey);
            var computedSignature = hmac.ComputeHash(Encoding.ASCII.GetBytes(candidate));

            var signaturesMatch = computedSignature.SequenceEqual(receivedSignature);
            return signaturesMatch;
        }
    }
}
