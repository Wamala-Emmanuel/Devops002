using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.DTOs
{
    public class VerificationPendingResponse : PendingResponse
    {
        /// <summary>
        /// Status of the request sent to verify
        /// </summary>
        public RequestStatus Status { get; set; }

#nullable enable
        public VerificationPendingResponse(Guid requestId, ApiVersion? version)
        {
            Id = requestId;
            RequestUri = $"/api/requests/{requestId}?v={version?.ToString()}";
            Status = RequestStatus.Pending;
        }
#nullable disable
    }
}
