using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services.Nira;
using GatewayService.Services.Nita.Json;
using GatewayService.Services.NotifierService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NiraWebService;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class NiraServiceTests
    {
        private readonly Mock<INitaJsonService> _mockNitaJsonService;
        private readonly Mock<INiraCoreService> _mockNiraCoreService;
        private readonly Mock<INotifierService> _mockNotifierService;
        private readonly Mock<ICredentialRepository> _mockCredentialRepository;
        private readonly Mock<IRequestRepository> _mockRequestRepository;
        private readonly NiraService _service;
        private readonly NiraConfig _niraConfig;
        private readonly Mock<IOptions<VerificationSettings>> _verificationOptions;
        private readonly VerificationSettings _verificationSettings = TestHelper.GetVerificationSettings();
        private readonly ILogger<NiraService> _logger;
        private readonly PersonInfoVerificationResponse _niraVerificationResponse;
        private readonly ChangePasswordResponse _niraOkPasswordResponse;
        private readonly ChangePasswordResponse _niraErrorPasswordResponse;
        private readonly NationalIdVerificationRequest _request;
        private readonly Models.Request _savedRequest;
        private readonly Credential _latestCredentials;
        private readonly Guid _requestId = Guid.NewGuid();
        private readonly string _username = "Emata@ROOT";

        private readonly Guid _credentialsId = Guid.NewGuid();

        public NiraServiceTests(ITestOutputHelper testOutputHelper)
        {
            _request = TestHelper.GetTestVerificationRequest();
            _requestId = Guid.NewGuid();

            _savedRequest = new Models.Request
            {
                CardNumber = _request.CardNumber,
                DateOfBirth = _request.DateOfBirth ?? DateTime.UtcNow,
                Id = _requestId,
                Surname = _request.Surname,
                GivenNames = _request.GivenNames,
                Nin = _request.Nin,
                ReceivedAt = DateTime.UtcNow,
                RequestStatus = RequestStatus.Pending,
            };

            _mockNitaJsonService = new Mock<INitaJsonService>();

            _mockNiraCoreService = new Mock<INiraCoreService>();

            _mockNotifierService = new Mock<INotifierService>();

            _mockCredentialRepository = new Mock<ICredentialRepository>();
            
            _mockRequestRepository = new Mock<IRequestRepository>();

            _logger = testOutputHelper.BuildLoggerFor<NiraService>();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables().Build();

            _niraConfig = config.GetNiraSettings();

            _niraVerificationResponse = new PersonInfoVerificationResponse
            {
                CardStatus = "Valid",
                ExecutionCost = "0.0",
                MatchingStatus = "true",
                PasswordDaysLeft = "99",
                Status = "Ok",
                IsError = false
            };

            _niraOkPasswordResponse = new ChangePasswordResponse
            {
                ExecutionCost = "0.0",
                PasswordDaysLeft = "199",
                Status = "Ok",
                IsError = false
            };

#nullable disable
            _niraErrorPasswordResponse = new ChangePasswordResponse
            {
                Status = "Error",
                IsError = true,
                Error = new ResponseError
                {
                    Code = "206",
                    Message = $"The Username/Password token for user {_username} has expired!"
                },
                PasswordDaysLeft = null
            };
#nullable enable

            _verificationOptions = new Mock<IOptions<VerificationSettings>>();

            _verificationOptions.Setup(x => x.Value)
                .Returns(_verificationSettings);

            _latestCredentials = TestHelper.GetLatestTestCredentials();
            
            _mockCredentialRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_latestCredentials);

            _mockCredentialRepository.Setup(x => x.FindAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_latestCredentials);
           
            _mockRequestRepository.Setup(mr => mr.FindAsync(
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_savedRequest);

            _mockRequestRepository.Setup(mr => mr.UpdateAsync(
                It.IsAny<Models.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_savedRequest);

            _mockNotifierService.Setup(ms => ms.PublishRequest(It.IsAny<Models.Request>())).Verifiable();

            _mockNiraCoreService.Setup(
                ms => ms.VerifyPersonInformation(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<verifyPersonInformationRequest>())).ReturnsAsync(_niraVerificationResponse);

            _mockNiraCoreService.Setup(ms => ms.ChangePasswordAsync(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<changePasswordRequest>())).ReturnsAsync(_niraOkPasswordResponse);

            _mockNitaJsonService.Setup(
                ms => ms.MakeJsonRequest(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<verifyPersonInformationRequest>())).ReturnsAsync(_niraVerificationResponse);

            _mockNitaJsonService.Setup(ms => ms.MakeJsonRequest(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<changePasswordRequest>())).ReturnsAsync(_niraOkPasswordResponse);
            
            _service = new NiraService(_mockNiraCoreService.Object, _mockCredentialRepository.Object,
                _mockRequestRepository.Object, config, _mockNitaJsonService.Object,
                _verificationOptions.Object, _mockNotifierService.Object, _logger);
        }

        [Fact]
        public async Task SendRequest_ShouldNotThrowException()
        {
            await _service.SendRequest(_requestId);
        }

        [Fact]
        public async Task SendRequest_ShouldFindRequest()
        {
            await _service.SendRequest(_requestId);

            _mockRequestRepository.Verify(
                mr => mr.FindAsync(It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task SendRequest_ShouldThrowsExceptionWhenRequestIsNull()
        {
            _mockRequestRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Models.Request)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.SendRequest(_requestId));
        }
#nullable enable

        [Fact]
        public async Task SendRequest_ShouldUseCredentialsFromAppSettings()
        {
            await _service.SendRequest(_requestId);

            _mockCredentialRepository.Verify(
                mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SendRequest_ShouldVerifyPersonInformationWithNitaConnection()
        {

            _verificationOptions.Setup(x => x.Value)
                .Returns( new VerificationSettings
                { 
                    ConnectionType = ConnectionType.Nita
                });

            await _service.SendRequest(_requestId);

            _mockNitaJsonService.Verify(
                ms => ms.MakeJsonRequest(
                        It.Is<string>(x => x == _niraConfig.CredentialConfig.Username),
                        It.Is<string>(x => x == _niraConfig.CredentialConfig.Password),
                        It.IsAny<verifyPersonInformationRequest>()), Times.Once);

        }

        [Fact]
        public async Task SendRequest_ShouldVerifyPersonInformationNiraConnection()
        {
            await _service.SendRequest(_requestId);

            _mockNiraCoreService.Verify(
                ms => ms.VerifyPersonInformation(
                        It.Is<string>(x => x == _niraConfig.CredentialConfig.Username),
                        It.Is<string>(x => x == _niraConfig.CredentialConfig.Password),
                        It.IsAny<verifyPersonInformationRequest>()), Times.Once);

        }

        [Fact]
        public async Task SendRequest_ShouldUpdateRequest()
        {
            await _service.SendRequest(_requestId);

            _mockRequestRepository.Verify(
                mr => mr.UpdateAsync(It.Is<Models.Request>(x => x.Id == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task SendRequest_ShouldNotifyUpdateRequest()
        {
            await _service.SendRequest(_requestId);

            _mockNotifierService.Verify(
                ms => ms.PublishRequest(It.Is<Models.Request>(x => x.Id == _requestId)), Times.Once);
        }

        [Fact]
        public async Task RenewPassword_ShouldNotThrowException()
        {
            await _service.RenewPasswordAsync(_credentialsId);
        }


        [Fact]
        public async Task RenewPassword_ShouldVerifyPersonInformationWithNitaConnection()
        {

            _verificationOptions.Setup(x => x.Value)
                .Returns(new VerificationSettings
                {
                    ConnectionType = ConnectionType.Nita
                });

            await _service.RenewPasswordAsync(_credentialsId);

            _mockNitaJsonService.Verify(
                ms => ms.MakeJsonRequest(
                        It.Is<string>(x => x == _username),
                        It.IsAny<string>(),
                        It.IsAny<changePasswordRequest>()), Times.Once);

        }

        [Fact]
        public async Task RenewPassword_ShouldVerifyPersonInformationNiraConnection()
        {
            await _service.RenewPasswordAsync(_credentialsId);

            _mockNiraCoreService.Verify(
                ms => ms.ChangePasswordAsync(
                        It.Is<string>(x => x == _username),
                        It.IsAny<string>(),
                        It.IsAny<changePasswordRequest>()), Times.Once);

        }

        [Fact]
        public async Task RenewPassword_ShouldSaveCredential()
        {
            await _service.RenewPasswordAsync(_credentialsId);

            _mockCredentialRepository.Verify(
                mr => mr.AddAsync(It.IsAny<Credential>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task RenewPassword_ShouldNotSavedCredentialsWhenPasswordDaysPropertyIsNull()
        {
            _mockNiraCoreService.Setup(ms => ms.ChangePasswordAsync(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<changePasswordRequest>())).ReturnsAsync(_niraErrorPasswordResponse);

            await Assert.ThrowsAsync<ApplicationException>(() =>
                _service.RenewPasswordAsync(_credentialsId));

            _mockCredentialRepository.Verify(
                mr => mr.AddAsync(It.IsAny<Credential>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
