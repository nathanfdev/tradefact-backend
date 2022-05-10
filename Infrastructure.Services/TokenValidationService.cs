//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Threading.Tasks;
//using IdentityModel;
//using IdentityModel.Client;
//using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;


//namespace Importwise.Serverless.Shared.Services
//{
//    /*
//     * This is a GWT token validator. It is used by the B2cValidationAttribute filter to handle security cross-cutting concerns. 
//     * 
//     * Because filters seem to be not fully baked in Azure Functions yet, I opted to use an APIM policy to verify the GWT token prior 
//     * to hitting the API.
//     */
//    public class TokenValidationService : ITokenValidationService
//    {
//        private const string AuthorizationHeaderName = "authorization";
//        private const string BearerScheme = "bearer";
//        private const string NameClaimType = "displayName";
//        private const string RoleClaimType = "role";
//        private const string ScopeClaimType = "scp";
//        private const string SigningKeyUseType = "sig";

//        private readonly string _audience;
//        private readonly DiscoveryCache _discoveryCache;
//        private readonly string _scope;

//        public TokenValidationService(ISettingService settingService)
//        {
//            _discoveryCache = new DiscoveryCache(settingService.GetAuthorityUrl(),
//                                                 new DiscoveryPolicy
//                                                 {
//                                                     ValidateIssuerName = false,
//                                                     ValidateEndpoints = false
//                                                 });

//            _audience = settingService.GetApiApplicationId();
//            _scope = settingService.GetApiScopeName();
//        }

//        private string ExtractBearerToken(HttpRequest request)
//        {
//            if (request.Headers.TryGetValue(AuthorizationHeaderName, out var authorization))
//            {
//                var header = authorization.FirstOrDefault();
//                if (header != null && header.StartsWith(BearerScheme, StringComparison.OrdinalIgnoreCase))
//                {
//                    var token = header.Substring(BearerScheme.Length).Trim();
//                    return token;
//                }
//            }
//            return null;
//        }

//        private async Task<TokenValidationParameters> GetValidationParameters()
//        {
//            var disco = await _discoveryCache.GetAsync();
//            if (disco.IsError)
//                return null;

//            var keys = disco.KeySet.Keys
//                .Where(x => x.Use == SigningKeyUseType)
//                .Select(x => new RsaSecurityKey(new RSAParameters
//                {
//                    Exponent = Base64Url.Decode(x.E),
//                    Modulus = Base64Url.Decode(x.N)
//                })
//                {
//                    KeyId = x.Kid
//                });

//            return new TokenValidationParameters
//            {
//                ValidIssuer = disco.Issuer,
//                ValidAudience = _audience,
//                IssuerSigningKeys = keys,
//                NameClaimType = NameClaimType,
//                RoleClaimType = RoleClaimType
//            };
//        }

//        private async Task<ClaimsPrincipal> ValidateJwt(string token)
//        {
//            var validationParams = await GetValidationParameters();
//            if (validationParams == null)
//                return null;

//            var handler = new JwtSecurityTokenHandler();
//            handler.InboundClaimTypeMap.Clear();

//            try
//            {
//                var principal = handler.ValidateToken(token, validationParams, out _);
//                if (principal.HasClaim(ScopeClaimType, _scope))
//                    return principal;
//            }
//            catch (Exception ex)
//            {
//                //_loggerService.Log("Token failed to validate: {0}", ex.Message);
//            }

//            return null;
//        }

//        public async Task<ClaimsPrincipal> AuthenticateRequest(HttpRequest request)
//        {
//            var token = ExtractBearerToken(request);
//            if (token == null)
//                return null;
//            var principal = await ValidateJwt(token);
//            return principal;
//        }
//    }
//}

