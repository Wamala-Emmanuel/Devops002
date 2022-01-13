using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Hubs.Contracts;
using GatewayService.Hubs.Implementations;
using GatewayService.Services;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GatewayService.Helpers.Notification.Handlers
{
    public class RequestNotifier : INotificationHandler<RequestNotification>
    {
        private readonly IHubContext<RequestHub, IRequestClient> _hubContext;

        public RequestNotifier( IHubContext<RequestHub, 
            IRequestClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(RequestNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.ReceiveNinRequest(notification.RequestViewModel);
        }
    }
}
