﻿using System.Collections.Generic;
using System.Net;

namespace GatewayService.Helpers
{
#nullable disable
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } 
        public string Data { get; set; }
        public List<string> Errors { get; set; }    
    }

    public class ErrorResponse
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; } 
    }
}
