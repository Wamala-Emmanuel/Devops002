using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Helpers
{
    public class ProxySettings
    {
        public string Url { get; set; }
        public bool BypassLocal { get; set; }
        public bool Enabled { get; set; }
    }
}
