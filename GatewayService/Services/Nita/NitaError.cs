using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatewayService.Services.Nita
{
    /// <summary>
    /// Error from NITA. When isError is false, its null
    /// </summary>
    public class NitaError
    {
        /// <summary>
        /// The code of the error returned 
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// The message of the error returned
        /// </summary>
        [JsonProperty("message")] 
        public string Message { get; set; }
    }
}
