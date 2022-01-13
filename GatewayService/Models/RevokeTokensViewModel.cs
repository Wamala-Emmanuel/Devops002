using System.ComponentModel.DataAnnotations;

namespace GatewayService.Models
{
#nullable disable
    /// <summary>
    /// Revoke Tokens View Model
    /// </summary>
    public class RevokeTokensViewModel
    {
        /// <summary>
        /// Access Token to revoke
        /// </summary>
        [Required]
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token to revoke
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Client ID
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Client Secret
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }
    }
}