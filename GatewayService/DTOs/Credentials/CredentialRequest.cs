using System.ComponentModel.DataAnnotations;

namespace GatewayService.DTOs.Credentials
{
    public class CredentialRequest
    {
        /// <summary>
        /// Username of the participant from NIRA
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Password for the participant from NIRA
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
