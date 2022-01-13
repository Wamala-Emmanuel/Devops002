using System;
using System.Runtime.Serialization;
using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.DTOs
{
#nullable disable
    /// <summary>
    /// Verification request
    /// </summary>
    public class VerificationRequest : ActionResult
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Status of the request
        /// </summary>
        public RequestStatus Status { get; set; }

        /// <summary>
        /// Result of the ID verification
        /// </summary>
        public string Result { get; set; }

    }

}
