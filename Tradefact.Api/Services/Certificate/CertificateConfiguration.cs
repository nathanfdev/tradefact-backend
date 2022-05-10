using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Services.Certificate
{
    public class CertificateConfiguration
    {
        public bool UseLocalCertStore { get; set; }
        public string CertificateThumbprint { get; set; }
        public string CertificateNameKeyVault { get; set; }
        public string KeyVaultEndpoint { get; set; }
        public string CertificatePfx { get; set; }
        public string CertificatePassword { get; set; }
    }

    public class KeyVaultConfiguration
    {
        public string TenantId { get; set; }
        public string Endpoint { get; set; }
        public string AppClientId { get; set; }
        public string AppClientSecret { get; set; }
        public string CertificateName { get; set; }
    }

}
