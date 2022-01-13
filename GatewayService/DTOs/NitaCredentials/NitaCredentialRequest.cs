using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs.NitaCredentials
{
    public class NitaCredentialRequest
    {
        /// <summary>
        /// ClientKey of the participant from NITA
        /// </summary>
        [Required]
        public string ClientKey { get; set; }

        /// <summary>
        /// ClientSecret for the participant from NITA
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }
    }
}
