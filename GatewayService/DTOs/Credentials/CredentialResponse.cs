using System;

namespace GatewayService.DTOs.Credentials
{
    public class CredentialResponse
    {
        /// <summary>
        /// The Id of the credentials, if the credentials are set in the database
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Username of the participant from NIRA
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Date when the password is entered 
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Date when the password is expected to expire
        /// </summary>
        public DateTime? ExpiresOn { get; set; }

    }
}
