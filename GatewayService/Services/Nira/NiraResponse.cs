using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Services.Nira
{
    public class NiraResponse
    {
        public string Status { get; set; }
        public string PasswordDaysLeft { get; set; }
        public string ExecutionCost { get; set; }

        public ResponseError Error { get; set; }

        public bool IsError { get; set; }
    }
}
