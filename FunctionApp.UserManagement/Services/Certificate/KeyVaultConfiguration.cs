namespace Tradefact.UserManagement.Services.Certificate
{
    public class KeyVaultConfiguration
    {
        public string TenantId { get; set; }
        public string Endpoint { get; set; }
        public string AppClientId { get; set; }
        public string AppClientSecret { get; set; }
        public string CertificateName { get; set; }
    }

}
