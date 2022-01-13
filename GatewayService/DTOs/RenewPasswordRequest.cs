using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class RenewPasswordRequest
    {
        /// <summary>
        /// The new password for the participant
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }
}
