using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using MediatR;

namespace GatewayService.Helpers.Notification
{
    public class RequestNotification : INotification
    {
        public RequestViewModel RequestViewModel { get; set; }
    }
}
