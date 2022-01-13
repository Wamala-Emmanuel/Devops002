using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatewayService.Services.Nira
{

    /// <summary>
    /// Error from NIRA. When isError is false, its null
    /// </summary>
    public class ResponseError
    {
        /// <summary>
        /// The code of the error returned 
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The message of the error returned
        /// </summary>
        public string Message { get; set; }
    }
}
