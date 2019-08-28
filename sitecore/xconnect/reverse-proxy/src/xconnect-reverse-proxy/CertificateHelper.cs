using System.Security.Cryptography.X509Certificates;

namespace xConnectReverseProxy
{
    public static class CertificateHelper
    {
        public static X509Certificate2 GetCertificateFromStore(string connectionString)
        {
            CertificateOptions options = new CertificateOptions(connectionString);

            X509Certificate2 certificate = new X509Certificate2();

            X509Store store = new X509Store(options.StoreName, options.StoreLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(options.FindType, options.FindValue, !options.AllowInvalidClientCertificates);
            if (x509Certificate2Collection.Count > 0)
            {
                certificate = x509Certificate2Collection[0];
            };

            store.Close();

            return certificate;
        }
    }
}