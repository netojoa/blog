using System;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

namespace xConnectReverseProxy
{
    public class CertificateOptions
    {
        public bool AllowInvalidClientCertificates
        {
            get;
            set;
        }

        public string CertificateConnectionStringName
        {
            get;
            set;
        }

        public X509FindType FindType
        {
            get;
            set;
        }

        public string FindValue
        {
            get;
            set;
        }

        public StoreLocation StoreLocation
        {
            get;
            set;
        }

        public StoreName StoreName
        {
            get;
            set;
        }

        public CertificateOptions(string certificateConnectionStringName)
        {
            this.CertificateConnectionStringName = certificateConnectionStringName;
            ParseConnectionString();
        }

        protected void ParseConnectionString()
        {
            DbConnectionStringBuilder dbConnectionStringBuilders = new DbConnectionStringBuilder()
            {
                ConnectionString = this.CertificateConnectionStringName
            };

            if (dbConnectionStringBuilders.TryGetValue("StoreName", out object storeName))
            {
                this.StoreName = (StoreName)Enum.Parse(typeof(StoreName), (string)storeName, true);
            }

            if (dbConnectionStringBuilders.TryGetValue("StoreLocation", out object storeLocation))
            {
                this.StoreLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), (string)storeLocation, true);
            }

            if (dbConnectionStringBuilders.TryGetValue("FindType", out object findType))
            {
                this.FindType = (X509FindType)Enum.Parse(typeof(X509FindType), (string)findType, true);
            }

            if (dbConnectionStringBuilders.TryGetValue("FindValue", out object findValue))
            {
                this.FindValue = (string)findValue;
            }

            if (dbConnectionStringBuilders.TryGetValue("AllowInvalidClientCertificates", out object allowInvalidClientCertificate))
            {
                this.AllowInvalidClientCertificates = (bool)allowInvalidClientCertificate;
            }
        }
    }
}