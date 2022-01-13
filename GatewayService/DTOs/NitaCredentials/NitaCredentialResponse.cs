using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs.NitaCredentials
{
    public class NitaCredentialResponse
    {
        /// <summary>
        /// The Id of the NITA credentials, if the NITA credentials are set in the database
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// ClientKey of the participant from NITA
        /// </summary>
        public string ClientKey { get; set; }

        /// <summary>
        /// Date when the client credentials were added 
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Date when the client credentials were updated
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
