using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.Services.Nira
{
    public class ChangePasswordResponse : NiraResponse
    {
        public ChangePasswordResponse()
        {
            IsError = Status != "Ok";
        }
    }

}
