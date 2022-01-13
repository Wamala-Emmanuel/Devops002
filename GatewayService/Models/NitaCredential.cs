using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Models
{
    /// <summary>
    /// The NITA client credentials
    /// </summary>
    public class NitaCredential
    {
        /// <summary>
        /// ID of NITA Credentials
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ClientKey of the participant from NITA
        /// </summary>
        public string ClientKey { get; set; }

        /// <summary>
        /// ClientSecret of the participant from NITA
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// When the NITA Credentials are entered
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// When the NITA Credentials are updated
        /// </summary>
        public DateTime? UpdatedOn { get; set; }
    }
}
