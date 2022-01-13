using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Helpers.Mappers;
using GatewayService.Helpers.Nira;
using GatewayService.Helpers.Notification;
using GatewayService.Models;
using MediatR;
using Newtonsoft.Json;

namespace GatewayService.Services.NotifierService
{
    public class NotifierService : INotifierService
    {
        private readonly IMediator _mediator;

        public NotifierService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void PublishCredentials(CredentialResponse response)
        {
            _mediator.Publish(new CredentialNotification
            {
                CredentialResponse = response
            });
        }

        public void PublishRequest(Request request)
        {
            _mediator.Publish(new RequestNotification 
            { 
                RequestViewModel = MapperProfiles.MapRequestModelToRequestViewModel(request)
            });
        }
    }
}
