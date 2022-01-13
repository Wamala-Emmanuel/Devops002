using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Services.Nira;
using Newtonsoft.Json;

namespace GatewayService.Services.Nita
{
    public class NitaResponse
    {
        [JsonProperty("return")]
        public NitaReturn Return { get; set; }
    }
}
