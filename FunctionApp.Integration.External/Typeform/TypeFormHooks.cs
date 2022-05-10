using System;
using System.Security.Cryptography;
using System.Text;

namespace FunctionApp.Integration.External.Typeform
{
    public partial class TypeFormHooks
    {
        private static string secret = Environment.GetEnvironmentVariable("TypeformSecret", EnvironmentVariableTarget.Process);
        public static string GenerateSignature(string payload, string secret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            byte[] queryStringBytes = Encoding.UTF8.GetBytes(payload);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
            byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);

            return Convert.ToBase64String(bytes);
        }

        private bool IsValidTypeformSignature(string payload, string signature, string secret)
        {
            string generatedSig = $"sha256={GenerateSignature(payload, secret)}";
            return (generatedSig == signature);
        }

    }
}
