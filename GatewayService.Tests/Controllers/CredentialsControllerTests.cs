using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Controllers;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Controllers
{
    public class CredentialsControllerTests : BaseControllerTests
    {
        private readonly Mock<ICredentialService> _serviceMock;
        private readonly Mock<IBackgroundJobClient> _jobMock;
        private readonly ILogger<CredentialService> _logger;
        private readonly RenewPasswordRequest _passwordRequest = TestHelper.GetTestRenewPasswordRequest();
        private readonly CredentialRequest _credentialRequest = TestHelper.GetTestCredentialRequest();
        private readonly CredentialsController _controller;
        private readonly Credential _request;
        private readonly CredentialResponse _currentCredentials = new CredentialResponse
        {
            Username = "nira_username",
            Id = Guid.NewGuid(),
            ExpiresOn = DateTime.Today.AddDays(10)
        };

        public CredentialsControllerTests(ITestOutputHelper testOutputHelper)
        {
            _request = TestHelper.GetLatestTestCredentials();

            var requestList = new List<Credential>() { _request };

            _serviceMock = new Mock<ICredentialService>();

            _jobMock = new Mock<IBackgroundJobClient>();

            _logger = testOutputHelper.BuildLoggerFor<CredentialService>();

            _controller = new CredentialsController(_serviceMock.Object, _logger, _jobMock.Object)
            {
                ControllerContext = controllerContext
            };

            _serviceMock.Setup(x => x.SetCredentialsAsync(It.IsAny<CredentialRequest>()))
                .ReturnsAsync(_currentCredentials);

            _serviceMock.Setup(x => x.GetAllCredentialsAsync()).ReturnsAsync(requestList);

            _serviceMock.Setup(x => x.GetCurrentCredentialsAsync()).ReturnsAsync(_currentCredentials);

        }

        [Fact]
        public async Task SetCredentials_ShouldNotThrowExceptionAsync()
        {
           await _controller.SetCredentials(_credentialRequest);
        }

        [Fact]
        public async Task SetCredentials_ShouldCallService()
        {
            await _controller.SetCredentials(_credentialRequest);

            _serviceMock.Verify(x => x.SetCredentialsAsync(
                It.Is<CredentialRequest>(x =>
               x.Username == _credentialRequest.Username &&
               x.Password == _credentialRequest.Password)), Times.Once);
        }

        [Fact]
        public async Task SetCredentials_ShouldReturnResponseAndValidStatusAsync()
        {
            var response = await _controller.SetCredentials(_credentialRequest);

            Assert.NotNull(response);
            Assert.IsType<ActionResult<CredentialResponse>>(response);
        }

        [Fact]
        public async Task GetCurrent_ShouldNotThrowException()
        {
            await _controller.GetCurrent();
        }

        [Fact]
        public async Task GetCurrent_ShouldCallService()
        {
            await _controller.GetCurrent();

            _serviceMock.Verify(x => x.GetCurrentCredentialsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCurrent_ShouldReturnResultsFromService()
        {
            var response = await _controller.GetCurrent();

            Assert.NotNull(response);
            Assert.IsType<ActionResult<CredentialResponse>>(response);
        }

        [Fact]
        public async Task GetCurrent_ShouldReturnNotFound_WhenNotFoundExceptionIsThrown()
        {
            _serviceMock.Setup(x => x.GetCurrentCredentialsAsync())
                .ThrowsAsync(new NotFoundException(""));

            var response = await _controller.GetCurrent();

            Assert.NotNull(response);
            Assert.IsType<NotFoundObjectResult>(response.Result);
        }
    }
}
