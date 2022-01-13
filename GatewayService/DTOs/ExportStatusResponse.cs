using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.DTOs
{
    public class ExportStatusResponse : PendingResponse
    {

#nullable enable
        public ExportStatusResponse()
        {
        }
        
        public ExportStatusResponse(Guid requestId, ApiVersion? version, ExportStatus status)
        {
            Id = requestId;
            RequestUri = $"/api/exports/{requestId}/status?v={version?.ToString()}";
            Status = status;
        }
#nullable disable

        /// <summary>
        /// Status of the request
        /// </summary>
        public ExportStatus Status { get; set; }

#nullable enable
        /// <summary>
        /// Reason for the request failing 
        /// </summary>
        public string? Error { get; set; }
#nullable disable

    }
}
