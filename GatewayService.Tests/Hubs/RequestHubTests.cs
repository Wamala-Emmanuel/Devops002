using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Hubs.Contracts;
using GatewayService.Hubs.Implementations;
using GatewayService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using SignalR_UnitTestingSupportXUnit.Hubs;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Hubs
{
    public class RequestHubTests : HubUnitTestsBase<IRequestClient>
    {
        private readonly Mock<IVerificationRequestService> _verificationServiceMock;
        private readonly ILogger<VerificationRequestService> _logger;
        private readonly RequestHub _requestHub;
        
        public RequestHubTests(ITestOutputHelper testOutputHelper)
        {
            _verificationServiceMock = new Mock<IVerificationRequestService>();
            _logger = testOutputHelper.BuildLoggerFor<VerificationRequestService>();
            
            _verificationServiceMock.Setup(x => x.GetRequestsAsync(It.IsAny<SearchRequest>()));

            _requestHub = new RequestHub(_verificationServiceMock.Object, _logger);
        }

        [Fact]
        public async Task RequestForSearchResponse_ShouldNotThrowExceptionAsync()
        {
            AssignToHubRequiredProperties(_requestHub);

            await _requestHub.GetNinRequests();
        }

        [Fact]
        public async Task SearchRequestsAsync_ShouldCallService()
        {
            AssignToHubRequiredProperties(_requestHub);
            
            await _requestHub.GetNinRequests();

            _verificationServiceMock.Verify(
                x => x.GetRequestsAsync(It.IsAny<SearchRequest>()),
                Times.Once);
        }

        [Fact]
        public async Task ReceiveSearchResponse_Should_SendReceivedMessagetAsync()
        {
            AssignToHubRequiredProperties(_requestHub);

            await _requestHub.GetNinRequests();

            ClientsAllMock.Verify(
                c => c.ReceiveNinRequests(It.IsAny<SearchResponse>()), Times.Once());
        }
    }
}
