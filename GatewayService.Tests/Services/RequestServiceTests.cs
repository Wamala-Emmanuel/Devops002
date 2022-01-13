using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services;
using GatewayService.Services.BillingService;
using GatewayService.Services.Nira;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NiraWebService;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
#nullable disable
    public class RequestServiceTests
    {
        private readonly ILogger<RequestService> _logger;
        private readonly Mock<IRequestRepository> _mockRepository;
        private readonly Mock<ICredentialService> _mockCredentialService;
        private readonly Mock<INiraService> _mockNiraService;
        private readonly Mock<ITokenUtil> _mockTokenUtil;
        private readonly Mock<IBackgroundJobClient> _jobMock;
        private readonly Mock<HttpRequest> _mockRequest;
        private readonly Mock<IOptions<NiraSettings>> _niraOptionsMock;
        private readonly Mock<IOptions<AuthServiceSettings>> _authOptionsMock;
        private readonly Mock<IOptions<SubscriptionSettings>> _subOptionsMock;
        private readonly Guid _requestId;
        private readonly NationalIdVerificationRequest _request;
        private readonly Models.Request _savedRequest;
        private readonly RequestService _requestService;
        private readonly AuthServiceSettings _authSettings = TestHelper.GetAuthSettings();
        private readonly NiraSettings _niraSettings = TestHelper.GetNiraSettings();
        private readonly NiraSettings _otherNiraSettings = TestHelper.GetOtherNiraSettings();
        private readonly SubscriptionSettings _subSettings = TestHelper.GetSubscriptionSettings();

        public RequestServiceTests(ITestOutputHelper testOutputHelper)
        {
            _request = TestHelper.GetTestVerificationRequest();

            _requestId = Guid.NewGuid();

            _mockRequest = TestHelper.CreateMockRequest(_request);

            _savedRequest = new Models.Request
            {
                CardNumber = _request.CardNumber,
                DateOfBirth = _request.DateOfBirth.Value,
                Id = _requestId,
                Surname = _request.Surname,
                GivenNames = _request.GivenNames,
                Nin = _request.Nin,
                ReceivedAt = DateTime.UtcNow,
                RequestStatus = RequestStatus.Pending,
            };

            _logger = testOutputHelper.BuildLoggerFor<RequestService>();

            _jobMock = new Mock<IBackgroundJobClient>();

            _mockRepository = new Mock<IRequestRepository>();

            _mockRepository.Setup(mr => mr.AddAsync(It.IsAny<Models.Request>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockCredentialService = new Mock<ICredentialService>();

            _niraOptionsMock = new Mock<IOptions<NiraSettings>>();

            _niraOptionsMock.Setup(x => x.Value)
                .Returns(_niraSettings);
            
            _subOptionsMock = new Mock<IOptions<SubscriptionSettings>>();

            _subOptionsMock.Setup(x => x.Value)
                .Returns(_subSettings);

            _authOptionsMock = new Mock<IOptions<AuthServiceSettings>>();

            _authOptionsMock.Setup(x => x.Value)
                .Returns(_authSettings);

            _mockNiraService = new Mock<INiraService>();

            _mockNiraService.Setup(n => n.SendRequest(
                It.IsAny<Guid>()));

            _jobMock.Setup(x => x.Create(It.Is<Job>(
                job => job.Method.Name == nameof(INiraService.SendRequest)), It.IsAny<EnqueuedState>()))
                .Returns("1");

            _jobMock.Setup(x => x.Create(It.Is<Job>(
                job => job.Method.Name == nameof(IBillingService.UpdateBilling)), It.IsAny<AwaitingState>()))
                .Returns("2");
            
            _jobMock.Setup(x => x.Create(It.Is<Job>(
                job => job.Method.Name == nameof(ICredentialService.SchedulePasswordRenewalJobAsync)), It.IsAny<AwaitingState>()))
                .Returns("3");

            _mockTokenUtil = new Mock<ITokenUtil>();

            _requestService = new RequestService(_mockRepository.Object, _mockNiraService.Object,
                _niraOptionsMock.Object, _authOptionsMock.Object,
                _subOptionsMock.Object, _logger, _jobMock.Object, 
                _mockTokenUtil.Object, _mockCredentialService.Object);
        }

        [Fact]
        public async Task Service_ShouldNotThrowException()
        {
            await _requestService.Process(_request, _mockRequest.Object);
        }

        [Fact]

        public async Task Service_ShouldSaveRequest()
        {
            await _requestService.Process(_request, _mockRequest.Object);

            _mockRepository.Verify(
                mr => mr.AddAsync(It.IsAny<Models.Request>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Service_ShouldCallNiraService()
        {
            await _requestService.Process(_request, _mockRequest.Object);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y =>
                    y.Method.Name == nameof(INiraService.SendRequest)),
                        It.IsAny<EnqueuedState>()), Times.Once);

        }
        
        [Fact]
        public async Task Service_ShouldNotCallCredentialServiceWhenUseDatabaseCredentialsIsFalse()
        {
            await _requestService.Process(_request, _mockRequest.Object);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y =>
                    y.Method.Name == nameof(ICredentialService.SchedulePasswordRenewalJobAsync)),
                        It.IsAny<AwaitingState>()), Times.Never);

        }
        
        [Fact]
        public async Task Service_ShouldReturnRequestId()
        {
            var result = await _requestService.Process(_request, _mockRequest.Object);

            Assert.IsType<Guid>(result);
        }

    }
}
