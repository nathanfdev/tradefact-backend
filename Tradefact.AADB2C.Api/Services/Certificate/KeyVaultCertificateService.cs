using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Tradefact.AADB2C.Api.Services.Certificate
{
    public class KeyVaultCertificateService
    {
        private readonly KeyVaultConfiguration _keyVaultConfig;

        private CertificateClient GetCertClient => new CertificateClient(new Uri(_keyVaultConfig.Endpoint), new ClientSecretCredential(_keyVaultConfig.TenantId, _keyVaultConfig.AppClientId, _keyVaultConfig.AppClientSecret));

        public KeyVaultCertificateService(KeyVaultConfiguration keyVaultConfig)
        {
            if (string.IsNullOrEmpty(keyVaultConfig.Endpoint))
            {
                throw new ArgumentException("missing keyVaultEndpoint");
            }
            _keyVaultConfig = keyVaultConfig;
        }

        public async Task<X509Certificate2> GetCertificateFromKeyVault()
        {

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            CertificateClient client = GetCertClient;
            return await GetCertificateAsync(_keyVaultConfig.CertificateName, client);
        }


        private async Task<X509Certificate2> GetCertificateAsync(string identitifier, CertificateClient keyVaultClient)
        {
            Azure.Response<KeyVaultCertificateWithPolicy> response = await keyVaultClient.GetCertificateAsync(identitifier);
            var certificateWithPrivateKey = new X509Certificate2(response.Value.Cer, (string)null, X509KeyStorageFlags.MachineKeySet);
            return certificateWithPrivateKey;
        }

    }
}
