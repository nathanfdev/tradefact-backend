using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Core.Common
{
    public class InviteLink
    {
        public string InviteCompany;
        public string InviteEmail;
        public string InviteId;
        public string InviteType;
        public string Issuer;
        public string B2CSignUpUrl;
        public string B2CTenant;
        public string B2CPolicy;
        public string B2CClientId;
        public string B2CRedirectUri;

        public string GetLink()
        {
            var token = BuildIdToken();
            var nonce = Guid.NewGuid().ToString("n");

            return string.Format(B2CSignUpUrl,
                    B2CTenant,
                    B2CPolicy,
                    B2CClientId,
                    Uri.EscapeUriString(B2CRedirectUri),
                    nonce) + "&id_token_hint=" + token;
        }

        private string BuildIdToken()
        {
            var securityKey = Encoding.UTF8.GetBytes("2VK62QTn0m1hMcn0DQ3TRADArF4ct6yIiSvYgdRwjZtU5QhI=");

            var signingKey = new SymmetricSecurityKey(securityKey);
            SigningCredentials signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
            claims.Add(new System.Security.Claims.Claim("invite.email", InviteEmail, System.Security.Claims.ClaimValueTypes.String, Issuer));
            claims.Add(new System.Security.Claims.Claim("invite.id", InviteId, System.Security.Claims.ClaimValueTypes.String, Issuer));
            claims.Add(new System.Security.Claims.Claim("invite.company", InviteCompany, System.Security.Claims.ClaimValueTypes.String, Issuer));
            claims.Add(new System.Security.Claims.Claim("invite.type", InviteType, System.Security.Claims.ClaimValueTypes.String, Issuer));

            // Create the token
            JwtSecurityToken token = new JwtSecurityToken(
                    Issuer,
                    B2CClientId,
                    claims,
                    DateTime.Now,
                    DateTime.Now.AddDays(7),
                    signingCredentials);

            // Get the representation of the signed token
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

            return jwtHandler.WriteToken(token);
        }
    }
}
