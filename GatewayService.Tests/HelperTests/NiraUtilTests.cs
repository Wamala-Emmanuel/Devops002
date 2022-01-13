using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using GatewayService.Helpers;
using GatewayService.Helpers.Nira;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GatewayService.Tests.HelperTests
{
    public class NiraUtilTests
    {
        private readonly string _certificatePath;
        private readonly string _wrongPath = "nira.crt";
        private readonly string _username = "test";
        private readonly string _password = "test123";
        private readonly string _culture;
        private readonly double _offset;

        public NiraUtilTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables().Build();
            var settings = config.GetNiraSettings();

            _offset = settings.NiraDateTimeConfig.Offset;
            _culture = settings.NiraDateTimeConfig.Culture;
            _certificatePath = settings.CredentialConfig.CertificatePath;
        }

        [Fact]
        public void SetUsernameToken_ShouldNotThrowException()
        {
            var result = NiraUtils.SetUsernameToken(_username, _password, _offset, _culture);
        }
        
        [Fact]
        public void SetUsernameToken_ShouldReturnAResult()
        {
            var result = NiraUtils.SetUsernameToken(_username, _password, _offset, _culture);

            Assert.NotNull(result);
            Assert.IsType<NiraUtils.UsernameToken>(result);
        }

        [Fact]
        public void SetUsernameToken_ShouldThrowClientFriendlyExceptionWhenAnEmptyStringUsernameIsPassed()
        {
            var username = string.Empty;

            Assert.Throws<ClientFriendlyException>(() => NiraUtils.SetUsernameToken(username, _password, _offset, _culture));
        }
        
        [Fact]
        public void SetUsernameToken_ShouldThrowClientFriendlyExceptionWhenAnEmptyStringPasswordIsPassed()
        {
            var password = string.Empty;

            Assert.Throws<ClientFriendlyException>(() => NiraUtils.SetUsernameToken(_username, password, _offset, _culture));
        }

        [Fact]
        public void GetEncryptionCertificate_ShouldNotThrowNoException()
        {
            var result = NiraUtils.GetEncryptionCertificate(_certificatePath);
        }
        
        [Fact]
        public void GetEncryptionCertificate_ShouldReturnAResult()
        {
            var result = NiraUtils.GetEncryptionCertificate(_certificatePath);

            Assert.NotNull(result);
            Assert.IsType<X509Certificate2>(result);
        }

        [Fact]
        public void GetEncryptionCertificate_ShouldThrowClientFriendlyExceptionWhenCertificatePathIsFalse()
        {
            Assert.Throws<ClientFriendlyException>(() => NiraUtils.GetEncryptionCertificate(_wrongPath));
        }
        
        [Fact]
        public void EncryptWithRSA_ShouldThrowNoException()
        {
            var certificate = NiraUtils.GetEncryptionCertificate(_certificatePath);
            
            var result = NiraUtils.EncryptWithRSA(certificate, _password);
        }

        [Fact]
        public void EncryptWithRSA_ShouldReturnAResult()
        {
            var certificate = NiraUtils.GetEncryptionCertificate(_certificatePath);
            
            var result = NiraUtils.EncryptWithRSA(certificate, _password);

            Assert.NotNull(result);
            Assert.IsType<byte[]>(result);
        }

        [Fact]
        public void EncryptWithRSA_ShouldThrowClientFriendlyExceptionWhenDataIsAnEmptyString()
        {
            var certificate = NiraUtils.GetEncryptionCertificate(_certificatePath);
            var secretData = string.Empty;

            Assert.Throws<ClientFriendlyException>(() => NiraUtils.EncryptWithRSA(certificate, secretData));
        }

#nullable enable
        [Fact]
        public void SetUsernameToken_ShouldThrowClientFriendlyExceptionWhenUsernameIsNull()
        {
            string? username = null;

            Assert.Throws<ClientFriendlyException>(() => NiraUtils.SetUsernameToken(username!, _password, _offset, _culture));
        }
        
        [Fact]
        public void SetUsernameToken_ShouldThrowClientFriendlyExceptionWhenPasswordIsNull()
        {
            string? password = null;

            Assert.Throws<ClientFriendlyException>(() => NiraUtils.SetUsernameToken(_username, password!, _offset, _culture));
        }

        [Fact]
        public void EncryptWithRSA_ShouldThrowClientFriendlyExceptionWhenDataIsNull()
        {
            var certificate = NiraUtils.GetEncryptionCertificate(_certificatePath);
            string? secretData = null;

            Assert.Throws<ClientFriendlyException>(() => NiraUtils.EncryptWithRSA(certificate, secretData));
        }
#nullable disable

        [Fact]
        public void GetNiraPassword_ShouldNotThrowException()
        {
            NiraUtils.GetNiraPassword();
        }
        
        [Fact]
        public void GetNiraPassword_ShouldReturnAResult()
        {
            var result = NiraUtils.GetNiraPassword();

            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }
        
        [Fact]
        public void GetNiraPassword_ShouldReturnAPasswordWithLengthOfEight()
        {
            var result = NiraUtils.GetNiraPassword();
            var expected = 8;
           
            Assert.Equal(expected, result.Length);
        }


        [Fact]
        public void GetNiraPassword_ShouldReturnAPasswordMatchingTheNiraPasswordPolicy()
        {
            var result = NiraUtils.GetNiraPassword();

            var expected = @"[a - z A - Z 0-9 -+_!@#$%^&*.,?]";
           
            Assert.Matches(expected, result);
        }
        
        [Fact]
        public void GetNiraPassword_ShouldReturnAPasswordWithoutWhiteSpace()
        {
            var result = NiraUtils.GetNiraPassword();

            var expected = @"\s";
           
            Assert.DoesNotMatch(expected, result);
        }
 
    }
}
