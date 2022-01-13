using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.DTOs.NitaCredentials;
using GatewayService.Helpers;
using GatewayService.Models;
using GatewayService.Repositories.Contracts;
using GatewayService.Services.Nita.NitaCredentialService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class NitaCredentialServiceTests
    {
        private readonly NitaCredentialService _service;
        private readonly ILogger<NitaCredentialService> _logger;
        private readonly Mock<INitaCredentialRepository> _mockRepository;
        private readonly Mock<IOptions<NitaSettings>> _nitaOptionsMock;
        private readonly NitaSettings _nitaSettings = TestHelper.GetNitaSettings();
        private readonly NitaCredential _nitaCredentials;
        private readonly NitaCredentialRequest _nitaCredentialRequest;
        private readonly NitaCredentialResponse _niraCredentialResponse;
        private readonly Guid _requestId;

        public NitaCredentialServiceTests(ITestOutputHelper testOutputHelper)
        {
            _mockRepository = new Mock<INitaCredentialRepository>(); 
            
            _logger = testOutputHelper.BuildLoggerFor<NitaCredentialService>();
           
            _nitaCredentialRequest = TestHelper.GetTestNitaCredentialRequest();

            _niraCredentialResponse = new NitaCredentialResponse
            {
                ClientKey = _nitaCredentialRequest.ClientKey,
            };

            _nitaCredentials = TestHelper.GetTestNitaCredentials();

            _requestId = _nitaCredentials.Id;

            _nitaOptionsMock = new Mock<IOptions<NitaSettings>>();

            _nitaOptionsMock.Setup(x => x.Value)
                .Returns(_nitaSettings);

            _mockRepository.Setup(mr => mr.AddAsync(It.IsAny<NitaCredential>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_nitaCredentials);

            _mockRepository.Setup(mr => mr.UpdateAsync(It.IsAny<NitaCredential>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_nitaCredentials);

            _mockRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_nitaCredentials);

            _mockRepository.Setup(mr => mr.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<NitaCredential>() { _nitaCredentials });

            _mockRepository.Setup(mr => mr.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_nitaCredentials);

            _mockRepository.Setup(mr => mr.GetCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            _service = new NitaCredentialService(_mockRepository.Object,
                _nitaOptionsMock.Object, _logger);
        }

        [Fact]
        public async Task AreNitaCredentialsSet_ShouldNotThrowException()
        {
            await _service.AreNitaCredentialsSet();
        }

        [Fact]
        public async Task AreNitaCredentialsSet_ShouldGetCredentialCount()
        {
            await _service.AreNitaCredentialsSet();

            _mockRepository.Verify(mr => mr.GetCountAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AreNitaCredentialsSet_ShouldReturnTrueWhenCredentialsExist()
        {
            var response = await _service.AreNitaCredentialsSet();

            Assert.IsType<bool>(response);
            Assert.True(response);
        }

        [Fact]
        public async Task AreNitaCredentialsSet_ShouldReturnFalseWhenCredentialsDonotExist()
        {

            _mockRepository.Setup(mr => mr.GetCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            
            var response = await _service.AreNitaCredentialsSet();

            Assert.IsType<bool>(response);
            Assert.False(response);
        }

        [Fact]
        public async Task GetAllNitaCredentialsAsync_ShouldNotThrowException()
        {
            await _service.GetAllNitaCredentialsAsync();
        }
        
        [Fact]
        public async Task GetCurrentNitaCredentialsAsync_ShouldNotThrowException()
        {
            await _service.GetCurrentNitaCredentialsAsync();
        }

        [Fact]
        public async Task GetCurrentNitaCredentialsAsync_ShouldGetCurrentCredential()
        {
            await _service.GetCurrentNitaCredentialsAsync();

            _mockRepository.Verify(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task GetCurrentNitaCredentialsAsync_ShouldThrowNotFoundExceptionWithNoLatestCredential()
        {

            _mockRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((NitaCredential)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCurrentNitaCredentialsAsync());

        }
#nullable enable

        [Fact]
        public async Task GetCurrentNitaCredentialsAsync_ShouldReturnResponseForCurrentCredential()
        {
            var response = await _service.GetCurrentNitaCredentialsAsync();

            Assert.NotNull(response);
            Assert.IsType<NitaCredentialResponse>(response);
        }

        [Fact]
        public async Task GetLatestNitaCredentialsAsync_ShouldNotThrowException()
        {
            await _service.GetLatestNitaCredentialsAsync();
        }

        [Fact]
        public async Task GetLatestNitaCredentialsAsync_ShouldGetLatestCredential()
        {
            await _service.GetLatestNitaCredentialsAsync();

            _mockRepository.Verify(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

#nullable disable
        [Fact]
        public async Task GetLatestNitaCredentialsAsync_ShouldThrowNotFoundExceptionWithNoLatestCredential()
        {

            _mockRepository.Setup(mr => mr.GetLatestAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((NitaCredential)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetLatestNitaCredentialsAsync());

        }
#nullable enable

        [Fact]
        public async Task GetLatestNitaCredentialsAsync_ShouldReturnResponseForLatestCredential()
        {
            var response = await _service.GetLatestNitaCredentialsAsync();

            Assert.NotNull(response);
            Assert.IsType<NitaCredential>(response);
        }

        [Fact]
        public async Task SetNitaCredentials_ShouldNotThrowException()
        {
            await _service.SetNitaCredentialsAsync(_nitaCredentialRequest);
        }

        [Fact]
        public async Task SetNitaCredentialsAsync_ShouldSaveCredential()
        {
            await _service.SetNitaCredentialsAsync(_nitaCredentialRequest);

            _mockRepository.Verify(
                mr => mr.AddAsync(It.Is<NitaCredential>(x =>
                        x.ClientKey == _nitaCredentialRequest.ClientKey &&
                        x.ClientSecret == _nitaCredentialRequest.ClientSecret),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetNitaCredentialsAsync_ShouldReturnResponseForTheCredentialsCreated()
        {
            var response = await _service.SetNitaCredentialsAsync(_nitaCredentialRequest);

            Assert.NotNull(response);
            Assert.Equal(_nitaCredentialRequest.ClientKey, response.ClientKey);
        }

        [Fact]
        public async Task UpdateNitaCredentialsAsync_ShouldNotThrowException()
        {
            await _service.UpdateNitaCredentialsAsync(_requestId, _nitaCredentialRequest);
        }

        [Fact]
        public async Task UpdateNitaCredentialsAsync_ShouldSaveCredential()
        {
            await _service.UpdateNitaCredentialsAsync(_requestId, _nitaCredentialRequest);

            _mockRepository.Verify(
                mr => mr.UpdateAsync(It.Is<NitaCredential>(x =>
                        x.ClientKey == _nitaCredentialRequest.ClientKey &&
                        x.ClientSecret == _nitaCredentialRequest.ClientSecret),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateNitaCredentialsAsync_ShouldReturnResponseForTheCredentialsUpdated()
        {
            var response = await _service.UpdateNitaCredentialsAsync(_requestId, _nitaCredentialRequest);

            Assert.NotNull(response);
            Assert.Equal(_nitaCredentialRequest.ClientKey, response.ClientKey);
        }

    }
}
