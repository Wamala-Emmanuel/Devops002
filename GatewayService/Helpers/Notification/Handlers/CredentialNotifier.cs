using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.Hubs.Contracts;
using GatewayService.Hubs.Implementations;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace GatewayService.Helpers.Notification.Handlers
{
    public class CredentialNotifier : INotificationHandler<CredentialNotification>
    {
        private readonly IHubContext<CredentialHub, ICredentialClient> _hubContext;

        public CredentialNotifier(IHubContext<CredentialHub, ICredentialClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(CredentialNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.ReceiveCredentialUpdate(notification.CredentialResponse);
        }
    }
}
