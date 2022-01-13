using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs.Credentials;
using MediatR;

namespace GatewayService.Helpers.Notification
{
    public class CredentialNotification : INotification
    {
        public CredentialResponse CredentialResponse { get; set; }
    }
}
