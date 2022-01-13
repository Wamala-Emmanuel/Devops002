using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class NiraSettings
    {
        public const string ConfigurationName = "NiraConfig";
     
        public int MaxRetries { get; set; }
        public string Url { get; set; }
        public CredentialConfig CredentialConfig { get; set; }
        public NiraDateTimeConfig NiraDateTimeConfig { get; set; }
        public List<string> BillableErrorCodes { get; set; }
        
    }

    public class CredentialConfig
    {
        public string CertificatePath { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PasswordDaysLimit { get; set; }
        public int PasswordExpirationDays { get; set; }
        public int PasswordLifeSpan { get; set; }
        public bool UseDatabaseCredentials { get; set; }
    }
    
    public class NiraDateTimeConfig
    {
        public string Culture { get; set; }
        public double Offset { get; set; }
        public string DateFormat { get; set; }
        public string ExportDateFormat { get; set; }
        public string NiraDateFormat { get; set; }
    }
}
