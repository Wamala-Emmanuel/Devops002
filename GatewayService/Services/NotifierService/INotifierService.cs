using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs.Credentials;
using GatewayService.Helpers.Notification;
using GatewayService.Models;
using MediatR;

namespace GatewayService.Services.NotifierService
{
    public interface INotifierService
    {
        void PublishRequest(Request request);

        void PublishCredentials(CredentialResponse response);
    }

}
