using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Controllers;
using GatewayService.DTOs.NitaCredentials;
using GatewayService.Services.Nita.NitaCredentialService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Controllers
{
    public class NitaCredentialsControllerTests : BaseControllerTests
    {
        private readonly NitaCredentialsController _controller;
        private readonly Mock<INitaCredentialService> _serviceMock;
        private readonly ILogger<NitaCredentialsController> _logger;
        private readonly NitaCredentialRequest _credentialRequest;
        private readonly NitaCredentialResponse _credentialResponse;
        private readonly Guid _requestId = Guid.NewGuid();

        public NitaCredentialsControllerTests(ITestOutputHelper testOutputHelper)
        {
            _credentialRequest = TestHelper.GetTestNitaCredentialRequest();

            _credentialResponse = new NitaCredentialResponse
            {
                Id = _requestId,
                ClientKey = _credentialRequest.ClientKey,
                CreatedOn = DateTime.Today.AddDays(10)
            };

            _logger = testOutputHelper.BuildLoggerFor<NitaCredentialsController>();

            _serviceMock = new Mock<INitaCredentialService>();

            _controller = new NitaCredentialsController(_serviceMock.Object, _logger)
            {
                ControllerContext = controllerContext
            };

            _serviceMock.Setup(x => x.SetNitaCredentialsAsync(It.IsAny<NitaCredentialRequest>()))
                .ReturnsAsync(_credentialResponse);

            _serviceMock.Setup(x => x.UpdateNitaCredentialsAsync(It.IsAny<Guid>(), It.IsAny<NitaCredentialRequest>()))
                .ReturnsAsync(_credentialResponse);

            _serviceMock.Setup(x => x.GetCurrentNitaCredentialsAsync()).ReturnsAsync(_credentialResponse);
        }

        [Fact]
        public async Task Post_ShouldNotThrowExceptionAsync()
        {
            await _controller.PostAsync(_credentialRequest);
        }

        [Fact]
        public async Task Post_ShouldCallService()
        {
            await _controller.PostAsync(_credentialRequest);

            _serviceMock.Verify(x => x.SetNitaCredentialsAsync(
                It.Is<NitaCredentialRequest>( x => 
                x.ClientKey == _credentialRequest.ClientKey &&
                x.ClientSecret == _credentialRequest.ClientSecret)), Times.Once);
        }

        [Fact]
        public async Task Post_ShouldReturnResponseAndValidStatusAsync()
        {
            var response = await _controller.PostAsync(_credentialRequest);

            Assert.NotNull(response);
            Assert.IsType<ActionResult<NitaCredentialResponse>>(response);
        }

        [Fact]
        public async Task GetCurrent_ShouldNotThrowExceptionAsync()
        {
            await _controller.GetCurrent();
        }

        [Fact]
        public async Task GetCurrent_ShouldCallService()
        {
            await _controller.GetCurrent();

            _serviceMock.Verify(x => x.GetCurrentNitaCredentialsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCurrent_ShouldReturnResultsFromService()
        {
            var response = await _controller.GetCurrent();

            Assert.NotNull(response);
            Assert.IsType<ActionResult<NitaCredentialResponse>>(response);
        }

        [Fact]
        public async Task EditAsync_ShouldNotThrowExceptionAsync()
        {
            await _controller.EditAsync(_requestId, _credentialRequest);
        }

        [Fact]
        public async Task Edit_ShouldCallService()
        {
            await _controller.EditAsync(_requestId, _credentialRequest);

            _serviceMock.Verify(x => x.UpdateNitaCredentialsAsync(
                It.Is<Guid>(x => x == _requestId),
                It.Is<NitaCredentialRequest>(y =>
               y.ClientKey == _credentialRequest.ClientKey &&
               y.ClientSecret == _credentialRequest.ClientSecret)), Times.Once);
        }

        [Fact]
        public async Task Edit_ShouldReturnResponseAndValidStatusAsync()
        {
            var response = await _controller.EditAsync(_requestId, _credentialRequest);

            Assert.NotNull(response);
            Assert.IsType<ActionResult<NitaCredentialResponse>>(response);
        }
    }
}
