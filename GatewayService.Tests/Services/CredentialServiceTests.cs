using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services;
using GatewayService.Services.Nira;
using GatewayService.Services.NotifierService;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class CredentialServiceTests
    {
        private readonly ILogger<CredentialService> _logger;
        private readonly Mock<IBackgroundJobClient> _jobMock;
        private readonly Mock<ICredentialRepository> _mockCredentialRepository;
        private readonly Mock<IRequestRepository> _mockRequestRepository;
        private readonly Mock<IBackgroundJobWrapper> _mockBackgroundJobWrapper;
        private readonly Mock<INiraService> _mockNiraService;
        private readonly Mock<INotifierService> _mockNotifierService;
        private readonly Credential _latestCredentials;
        private readonly Credential _newCredentials;
        private readonly CredentialRequest _credentialRequest;
        private readonly CredentialResponse _credentialResponse;
        private readonly RenewPasswordRequest _passwordRequest;
        private readonly CredentialService _service;
        private readonly NationalIdVerificationRequest _request;
        private readonly Models.Request _savedRequest;
        private readonly Models.Request _failedRequest;
        private readonly Models.Request _erroredRequest;
        private readonly Mock<IOptions<NiraSettings>> _niraOptionsMock;
        private readonly NiraSettings _niraSettings = TestHelper.GetNiraSettings();
        private readonly string _username = "Emata@ROOT";
        private readonly string _password = "987Wkln";
        private readonly Guid _requestId = Guid.NewGuid();

        public CredentialServiceTests(ITestOutputHelper testOutputHelper)
        {
            _credentialRequest = TestHelper.GetTestCredentialRequest();

            _latestCredentials = TestHelper.GetLatestTestCredentials();

            _newCredentials = TestHelper.GetNewTestCredentials();

            _passwordRequest = TestHelper.GetTestRenewPasswordRequest();

            _logger = testOutputHelper.BuildLoggerFor<CredentialService>();

            _jobMock = new Mock<IBackgroundJobClient>();
            
            _mockNiraService = new Mock<INiraService>();

            _mockNotifierService = new Mock<INotifierService>();

            _mockCredentialRepository = new Mock<ICredentialRepository>();

            _mockRequestRepository = new Mock<IRequestRepository>();
            
            _mockBackgroundJobWrapper = new Mock<IBackgroundJobWrapper>();

            _credentialResponse = new CredentialResponse
            {
                Username = _credentialRequest.Username,
            };

            _request = TestHelper.GetTestVerificationRequest();

            var niraErrorResult = TestHelper.GetTestVerificationResult();

            var niraOkResult = TestHelper.GetTestVerificationResult();
            niraOkResult.Status = "Ok";
           
            _savedRequest = new Models.Request
            {
                CardNumber = _request.CardNumber,
                DateOfBirth = _request.DateOfBirth ?? DateTime.UtcNow,
                Id = _requestId,
                Surname = _request.Surname,
                GivenNames = _request.GivenNames,
                Nin = _request.Nin,
                ReceivedAt = DateTime.UtcNow,
                RequestStatus = RequestStatus.Completed,
                Result = JsonConvert.SerializeObject(niraOkResult)
            };

            _failedRequest = new Models.Request
            {
                CardNumber = _request.CardNumber,
                DateOfBirth = _request.DateOfBirth ?? DateTime.UtcNow,
                Id = _requestId,
                Surname = _request.Surname,
                GivenNames = _request.GivenNames,
                Nin = _request.Nin,
                ReceivedAt = DateTime.UtcNow,
                RequestStatus = RequestStatus.Failed,
            };
            
            _erroredRequest = new Models.Request
            {
                CardNumber = _request.CardNumber,
                DateOfBirth = _request.DateOfBirth ?? DateTime.UtcNow,
                Id = _requestId,
                Surname = _request.Surname,
                GivenNames = _request.GivenNames,
                Nin = _request.Nin,
                ReceivedAt = DateTime.UtcNow,
                RequestStatus = RequestStatus.Completed
            };

            _niraOptionsMock = new Mock<IOptions<NiraSettings>>();

            _niraOptionsMock.Setup(x => x.Value)
                .Returns(_niraSettings);

            _jobMock.Setup(x => x.Create(It.Is<Job>(
                job => job.Method.Name == nameof(INiraService.RenewPasswordAsync)), It.IsAny<ScheduledState>())).Returns("1");
            
            _mockBackgroundJobWrapper.Setup(x => x.DeleteJob(It.IsAny<string>())).Returns(true);

            _mockNiraService.Setup(n => n.RenewPasswordAsync(It.IsAny<Guid>()));

            _mockNotifierService.Setup(ms => ms.PublishCredentials(It.IsAny<CredentialResponse>())).Verifiable();

            _mockCredentialRepository.Setup(mr => mr.AddAsync(It.IsAny<Credential>(), It.IsAny<CancellationToken>())).ReturnsAsync(_latestCredentials);

            _mockCredentialRepository.Setup(mr => mr.UpdateAsync(It.IsAny<Credential>(), It.IsAny<CancellationToken>())).ReturnsAsync(_latestCredentials);

            _mockCredentialRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_latestCredentials);

            _mockCredentialRepository.Setup(mr => mr.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Credential>() { _latestCredentials });

            _mockRequestRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(_savedRequest);

            _mockCredentialRepository.Setup(x => x.AnyActiveCredentials(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            _service = new CredentialService(_mockNiraService.Object, _mockCredentialRepository.Object, _mockRequestRepository.Object,
                _niraOptionsMock.Object, _jobMock.Object, _logger, _mockNotifierService.Object, _mockBackgroundJobWrapper.Object);
        }

        #region SetCredentials
        [Fact]
        public async Task SetCredentials_ShouldNotThrowException()
        {
            await _service.SetCredentialsAsync(_credentialRequest);
        }

        [Fact]
        public async Task Service_ShouldSaveCredential()
        {
            await _service.SetCredentialsAsync(_credentialRequest);

            _mockCredentialRepository.Verify(
                mr => mr.AddAsync(It.Is<Credential>(x =>
                        x.Username == _credentialRequest.Username),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendRequest_ShouldNotifyUpdateRequest()
        {
            await _service.SetCredentialsAsync(_credentialRequest);

            _mockNotifierService.Verify(
                ms => ms.PublishCredentials(
                    It.Is<CredentialResponse>(x => 
                        x.Username == _credentialResponse.Username)), Times.Once);
        }

        [Fact]
        public async Task Service_ShouldReturnCredentialResponseForTheCredentialsCreated()
        {
            var response = await _service.SetCredentialsAsync(_credentialRequest);

            Assert.NotNull(response);
            Assert.Equal(_username, response.Username);
        }

        [Fact]
        public async Task SetCredentials_ShouldGetLatestCredentialsFromDatabase()
        {
            await _service.SetCredentialsAsync(_credentialRequest);

            _mockCredentialRepository.Verify(x => x.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetCredentials_ShouldDeleteJob_WhenCurrentCredentialsExistAndHaveJobId()
        {
            await _service.SetCredentialsAsync(_credentialRequest);

            _mockBackgroundJobWrapper.Verify(x => 
                    x.DeleteJob(It.Is<string>(y => y == TestHelper.GetLatestTestCredentials().JobId)),
                Times.Once);
        }

        [Fact]
        public async Task SetCredentials_ShouldNotDeleteJob_WhenCurrentCredentialsAreNullOrEmptyOrDoNotHaveJobId()
        {
            _mockCredentialRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Credential
                {
                    JobId = null
                });

            await _service.SetCredentialsAsync(_credentialRequest);

            _mockBackgroundJobWrapper.Verify(x =>
                    x.DeleteJob(It.Is<string>(y => y == TestHelper.GetLatestTestCredentials().JobId)),
                Times.Never);
        }
        #endregion

        [Fact]
        public async Task GetCredentials_ShouldNotThrowException()
        {
            await _service.GetAllCredentialsAsync();
        }

        [Fact]
        public async Task Service_ShouldGetCredential()
        {
            await _service.GetAllCredentialsAsync();

            _mockCredentialRepository.Verify(
                mr => mr.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldNotThrowException()
        {
            await _service.SchedulePasswordRenewalJobAsync(_requestId);
        }

        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldCallNiraService()
        {
            _mockCredentialRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_newCredentials);

            await _service.SchedulePasswordRenewalJobAsync(_requestId);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y =>
                   y.Method.Name == nameof(INiraService.RenewPasswordAsync)),
                        It.IsAny<ScheduledState>()), Times.Once);
        }

        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldFindVerificationRequest()
        {
            await _service.SchedulePasswordRenewalJobAsync(_requestId);

            _mockRequestRepository.Verify(
                mr => mr.FindAsync(It.Is<Guid>(x => x == _requestId), It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldThrowExceptionWhenVerificationRequestIsNull()
        {
            _mockRequestRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Models.Request)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.SchedulePasswordRenewalJobAsync(_requestId));
        }

        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldShouldThrowExceptionWhenLatestCredentialRequestIsNull()
        {
            _mockCredentialRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Credential)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.SchedulePasswordRenewalJobAsync(_requestId));

        }
        
        [Fact]
        public async Task GetLatestCredentials_ShouldShouldThrowExceptionWhenLatestCredentialRequestIsNull()
        {
            _mockCredentialRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>())).ReturnsAsync((Credential)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetLatestCredentialsAsync());

        }
#nullable enable

        [Fact(Skip = "The renewal of password will now update the credentials regardless of if the renewal is to be set now or later.")]
        public async Task SchedulePasswordRenewalJob_ShouldDoNotScheduleWhenVerificationRequestIsNotSuccessful()
        {
            _mockRequestRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(_failedRequest);

            await _service.SchedulePasswordRenewalJobAsync(_requestId);

            _jobMock.Verify(x => x.Create(
                It.Is<Job>(y =>
                   y.Type.Name == nameof(INiraService)
                   && y.Method.Name == nameof(INiraService.RenewPasswordAsync)
                   && y.Args.Count > 0),
                   It.IsAny<ScheduledState>()), Times.Never);
        }


        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldFindLatestCredential()
        {
            await _service.SchedulePasswordRenewalJobAsync(_requestId);

            _mockCredentialRepository.Verify(
                mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task SchedulePasswordRenewalJob_ShouldDoNotScheduleWhenLatestCredentialHasJobIdAndExpiresOnDate()
        {
            await _service.SchedulePasswordRenewalJobAsync(_requestId);

            _jobMock.Verify(x => x.Create(
               It.Is<Job>(y =>
                   y.Type.Name == nameof(INiraService)
                   && y.Method.Name == nameof(INiraService.RenewPasswordAsync)
                   && y.Args.Count > 0),
                       It.IsAny<ScheduledState>()), Times.Never);
        }

        [Fact]
        public async Task Service_ShouldGetLatestCredentials()
        {
            await _service.GetLatestCredentialsAsync();

            _mockCredentialRepository.Verify(
                mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #region AreDatabaseCredentialsSet Tests

        [Fact]
        public async Task AreDatabaseCredentialsSet_ShouldNotThrowError()
        {
            await _service.AreDatabaseCredentialsSet();
        }

        [Fact]
        public async Task AreDatabaseCredentialsSet_ShouldCallRepository()
        {
            await _service.AreDatabaseCredentialsSet();

            _mockCredentialRepository.Verify(x => x.AnyActiveCredentials(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AreDatabaseCredentialsSet_ShouldReturnRepositoryResult()
        {
            
            var response = await _service.AreDatabaseCredentialsSet();

            Assert.True(response);
        }

        #endregion

        #region AreConfigCredentialsSet Tests

        [Fact]
        public void AreConfigCredentialsSet_ShouldNotThrowException()
        {
            _service.AreConfigCredentialsSet();
        }

        [Fact]
        public void AreConfigCredentialsSet_ShouldCallOptions()
        {
            _service.AreConfigCredentialsSet();

            _niraOptionsMock.Verify(x => x.Value, Times.Once);
        }

        [Fact]
        public void AreConfigCredentialsSet_ShouldReturnValueBasedOnConfig()
        {
            var configSet = !string.IsNullOrEmpty(_niraSettings.CredentialConfig.Username)
                            && !string.IsNullOrEmpty(_niraSettings.CredentialConfig.Password);

            var areSet = _service.AreConfigCredentialsSet();

            Assert.Equal(configSet, areSet);
        }
        #endregion

        #region GetCurrentCredentials Tests

        [Fact]
        public async Task GetCurrentCredentials_ShouldNotThrowException()
        {
            await _service.GetCurrentCredentialsAsync();
        }

        [Fact]
        public async Task GetCurrentCredentials_ShouldReturnConfigCredentials_WhenCredentialsAreSetInConfig()
        {
            _niraOptionsMock.Setup(x => x.Value)
                .Returns(new NiraSettings
                {
                    CredentialConfig = new CredentialConfig
                    {
                        UseDatabaseCredentials = false,
                        Username = _username,
                        Password = _password
                    }
                });

            var response = await _service.GetCurrentCredentialsAsync();

            _mockCredentialRepository.Verify(x => x.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Never);

            Assert.NotNull(response);
            Assert.Equal(_username, response.Username);
            Assert.Null(response.Id);
        }

        [Fact]
        public async Task GetCurrentCredentials_ShouldReturnDatabaseCurrentCredentials_WhenCredentialsAreSetInDatabase()
        {
            _niraOptionsMock.Setup(x => x.Value)
                .Returns(new NiraSettings
                {
                    CredentialConfig = new CredentialConfig
                    {
                        UseDatabaseCredentials = true,
                        Username = "config_username",
                        Password = _password
                    }
                });

            var response = await _service.GetCurrentCredentialsAsync();

            _mockCredentialRepository.Verify(x => x.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.NotNull(response);
            Assert.NotEqual("config_username", response.Username);
            Assert.NotNull(response.Id);
            Assert.NotNull(response.CreatedOn);
            Assert.NotNull(response.ExpiresOn);
        }

        [Fact]
        public async Task GetCurrentCredentials_ShouldThrowNotFoundException_WhenCredentialsInDatabaseAreNotFound()
        {
            _niraOptionsMock.Setup(x => x.Value)
                .Returns(new NiraSettings
                {
                    CredentialConfig = new CredentialConfig
                    {
                        UseDatabaseCredentials = true,
                        Username = "config_username",
                        Password = _password
                    }
                });

#nullable disable
            _mockCredentialRepository.Setup(x => x.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Credential);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCurrentCredentialsAsync());
        }
        #endregion
    }
}
