using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Models
{
    public class Credential
    {
        /// <summary>
        /// ID of Credentials
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// When the password is entered
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// When the password is set to expire
        /// </summary>
        public DateTime? ExpiresOn { get; set; }

        /// <summary>
        /// The hangfire job to execute the next password renewal
        /// </summary>
        public string? JobId { get; set; }
            
        /// <summary>
        /// Username of the participant from nira
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the participant from nira
        /// </summary>
        [Encrypted]
        public string Password { get; set; }
    }
}
