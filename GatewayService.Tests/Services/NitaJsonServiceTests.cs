using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services.Nita.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NiraWebService;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class NitaJsonServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<INitaCredentialRepository> _mockRepository;
        private readonly Mock<IOptions<NitaSettings>> _nitaOptionsMock;
        private readonly NitaSettings _nitaSettings = TestHelper.GetNitaSettings();
        private readonly NitaCredential _nitaCredentials;
        private readonly ILogger<NitaJsonService> _logger;
        private readonly NitaJsonService _nitaJsonService;
        private readonly verifyPersonInformationRequest _verifyPersonInformationRequest;
        private readonly changePasswordRequest _changePasswordRequest;
        private readonly string _password = string.Empty;
        private readonly string _username = string.Empty;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public NitaJsonServiceTests(ITestOutputHelper testOutputHelper)
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();



            _mockRepository = new Mock<INitaCredentialRepository>();

            _logger = testOutputHelper.BuildLoggerFor<NitaJsonService>();

            _nitaCredentials = new NitaCredential
            {
                ClientKey = string.Empty,
                ClientSecret = string.Empty
            };

            _verifyPersonInformationRequest = TestHelper.GetTestVerifyPersonInformationRequest();

            //_verifyPersonInformationRequest = TestHelper.GetTestDeceasedVerifyPersonInformationRequest();

            //_verifyPersonInformationRequest = TestHelper.GetTestFailedVerifyPersonInformationRequest();

            _changePasswordRequest = TestHelper.GetTestChangePasswordRequest();

            _nitaOptionsMock = new Mock<IOptions<NitaSettings>>();

            _nitaOptionsMock.Setup(x => x.Value)
                .Returns(_nitaSettings);

            _mockRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_nitaCredentials);

            _nitaJsonService = new NitaJsonService(_mockRepository.Object,
                _nitaOptionsMock.Object, 
                _mockHttpClientFactory.Object,
                _logger);
        }

        [Fact(Skip = "Not needed")]
        public async Task GetAccessToken_ShouldNotThrowException()
        {
            await _nitaJsonService.GetAccessToken();
        }

        [Fact(Skip = "Not needed")]
        public async Task VerifyPersonRequest_ShouldNotThrowException()
        {
            await _nitaJsonService.MakeJsonRequest(_username, _password, _verifyPersonInformationRequest);
        }

        [Fact(Skip = "Not needed")]
        public async Task ChangePasswordRequest_ShouldNotThrowException()
        {
            await _nitaJsonService.MakeJsonRequest(_username, _password, _changePasswordRequest);
        }
    }
}
