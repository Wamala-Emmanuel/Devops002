using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models
{
#nullable disable
    /// <summary>
    /// Login ViewModel
    /// </summary>
    public class UserLoginViewModel
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Client Secret
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }

        /// <summary>
        /// The resources the client wants to access
        /// <example>openid, profile, offline_access</example>
        /// </summary>
        public List<string> RequestingAccessTo { get; set; }
    }
}