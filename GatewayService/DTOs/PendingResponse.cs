using System;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.DTOs
{
    public class PendingResponse : ActionResult
    {
        /// <summary>
        /// Unique Id of the request
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Url to find the request
        /// </summary>
        public string RequestUri { get; set; }
    }
}
