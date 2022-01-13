using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class ClientCredentialsSettings
    {
        public const string SectionName = "ClientCredentials";

        public string AuthDefinition { get; set; }

        public string Description { get; set; }
        
        public string HeaderName { get; set; }
        
        public string TokenUrl { get; set; }

        public string ClientDefinition { get; set; }

        public List<Scope> Scopes { get; set; }

    }

    public class Scope
    {
        public string Name { get; set; }
        
        public string Description { get; set; }

    }
}
