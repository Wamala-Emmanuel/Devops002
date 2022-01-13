using System;
using System.Threading;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Helpers.Nira;
using GatewayService.Helpers.Notification;
using GatewayService.Models;
using GatewayService.Services.NotifierService;
using MediatR;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GatewayService.Tests.Services
{
    public class NotifierServiceTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly NotifierService _service;
        private readonly Request _request = TestHelper.GetTestRequest();
        private readonly CredentialResponse _response;

        public NotifierServiceTests()
        {
            var resultJson = JsonConvert.DeserializeObject<VerificationResultViewModel>(_request.Result ?? "{}",
                         new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            var viewModel = new RequestViewModel
            {
                Id = _request.Id,
                Initiator = _request.Initiator ?? null,
                InitiatorId = _request.InitiatorId ?? null,
                InitiatorEmail = _request.InitiatorEmail ?? null,
                ParticipantId = _request.ParticipantId ?? null,
                ReceivedAt = _request.ReceivedAt,
                SubmittedAt = _request.SubmittedAt ?? null,
                ReceivedFromNira = _request.ReceivedFromNira ?? null,
                RequestStatus = _request.RequestStatus,
                ResultJson = resultJson,
                Surname = _request.Surname,
                GivenNames = _request.GivenNames,
                CardNumber = _request.CardNumber,
                MaskedCardNumber = NiraUtils.MaskCardDetails(_request.CardNumber, charsToShow: 5),
                Nin = _request.Nin,
                MaskedNin = NiraUtils.MaskCardDetails(_request.Nin, charsToShow: 6),
                DateOfBirth = _request.DateOfBirth ?? null
            };

            _response = new CredentialResponse
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.Now.AddDays(-79),
                ExpiresOn = DateTime.Now.AddDays(-20),
                Username = "Emata@BankROOT"
            };
            
            _mockMediator = new Mock<IMediator>();

            _mockMediator.Setup(m => m.Publish(It.IsAny<RequestNotification>(), It.IsAny<CancellationToken>())).Verifiable();

            _mockMediator.Setup(m => m.Publish(It.IsAny<CredentialNotification>(), It.IsAny<CancellationToken>())).Verifiable();

            _service = new NotifierService(_mockMediator.Object);
        }

        [Fact]
        public void PublishRequest_ShouldNotThrowException()
        {
            _service.PublishRequest(_request);
        }   

        [Fact]
        public void PublishRequest_ShouldPublishNotification()
        {
            _service.PublishRequest(_request);

            _mockMediator.Verify(m => m.Publish( 
                It.Is<RequestNotification>( x => 
                x.RequestViewModel.Id == _request.Id && 
                x.RequestViewModel.Initiator == _request.Initiator && 
                x.RequestViewModel.InitiatorEmail == _request.InitiatorEmail &&
                x.RequestViewModel.GivenNames == _request.GivenNames && 
                x.RequestViewModel.Surname == _request.Surname && 
                x.RequestViewModel.Nin == _request.Nin && 
                x.RequestViewModel.CardNumber == _request.CardNumber && 
                x.RequestViewModel.ReceivedAt == _request.ReceivedAt && 
                x.RequestViewModel.SubmittedAt == _request.SubmittedAt && 
                x.RequestViewModel.ReceivedFromNira == _request.ReceivedFromNira ), 
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void PublishCredentials_ShouldNotThrowException()
        {
            _service.PublishCredentials(_response);
        }

        [Fact]
        public void PublishCredentials_ShouldPublishNotification()
        {
            _service.PublishCredentials(_response);

            _mockMediator.Verify(m => m.Publish(
                It.Is<CredentialNotification>(x => 
                x.CredentialResponse.Id == _response.Id && 
                x.CredentialResponse.CreatedOn == _response.CreatedOn && 
                x.CredentialResponse.ExpiresOn == _response.ExpiresOn && 
                x.CredentialResponse.Username == _response.Username),
                It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
