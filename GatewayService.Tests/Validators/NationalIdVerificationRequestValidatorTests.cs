using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using GatewayService.DTOs;
using GatewayService.Services;
using GatewayService.Services.Nita.NitaCredentialService;
using GatewayService.Validators;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GatewayService.Tests.Validators
{
#nullable disable
    public class NationalIdVerificationRequestValidatorTests
    {
        private readonly Mock<IOptions<NiraSettings>> _niraSettingsMock;
        private readonly Mock<ICredentialService> _credentialServiceMock;
        private readonly Mock<INitaCredentialService> _nitaCredentialServiceMock;
        private readonly NationalIdVerificationRequestValidator _validator;
        private readonly Mock<IOptions<NiraSettings>> _niraOptionsMock;
        private readonly Mock<IOptions<VerificationSettings>> _verificationOptionsMock;
        private readonly NiraSettings _niraSettings = TestHelper.GetNiraSettings();

        public NationalIdVerificationRequestValidatorTests()
        {
            _niraSettingsMock = new Mock<IOptions<NiraSettings>>();

            _credentialServiceMock = new Mock<ICredentialService>();
            
            _niraOptionsMock = new Mock<IOptions<NiraSettings>>();

            _verificationOptionsMock = new Mock<IOptions<VerificationSettings>>();

            _nitaCredentialServiceMock = new Mock<INitaCredentialService>();

            _niraOptionsMock.Setup(x => x.Value)
                .Returns(_niraSettings);

            _verificationOptionsMock.Setup(x => x.Value)
                .Returns(new VerificationSettings
                {
                    ConnectionType = ConnectionType.Nira
                });

            _validator = new NationalIdVerificationRequestValidator(_niraSettingsMock.Object, 
                _credentialServiceMock.Object, _nitaCredentialServiceMock.Object,
                _niraOptionsMock.Object, _verificationOptionsMock.Object);

            _niraSettingsMock.Setup(x => x.Value)
                .Returns(TestHelper.GetNiraSettings());
            _credentialServiceMock.Setup(x => x.AreConfigCredentialsSet())
                .Returns(true);
            _credentialServiceMock.Setup(x => x.AreDatabaseCredentialsSet())
                .ReturnsAsync(true);
            _credentialServiceMock.Setup(x => x.GetCurrentCredentialsAsync())
                .ReturnsAsync(new DTOs.Credentials.CredentialResponse
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now.AddDays(-25),
                    ExpiresOn = DateTime.Now.AddDays(20),
                    Username = "Emata@BankROOT"
                });

            _nitaCredentialServiceMock.Setup(x => x.AreNitaCredentialsSet())
                .ReturnsAsync(true);
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenNinIsNullOrEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(m => m.Nin, null as string)
                .WithErrorCode("NationalIdVerificationRequest.Nin.NullOrEmpty");
            _validator.ShouldHaveValidationErrorFor(m => m.Nin, string.Empty)
                .WithErrorCode("NationalIdVerificationRequest.Nin.NullOrEmpty");
        }
        
        [Fact]
        public void Validator_ShouldHaveErrorWhenNinLengthIsLessThan14()
        {
            _validator.ShouldHaveValidationErrorFor(m => m.Nin, "CMD124")
                .WithErrorCode("NationalIdVerificationRequest.Nin.ExactLengthValidator");
        }
        
        [Fact]
        public void Validator_ShouldHaveErrorWhenNinLengthIsMoreThan14()
        {
            _validator.ShouldHaveValidationErrorFor(m => m.Nin, "CMD1240588012VTHUE")
                .WithErrorCode("NationalIdVerificationRequest.Nin.ExactLengthValidator");
        }
        
        [Fact]
        public void Validator_ShouldHaveErrorWhenCardNumberIsNullOrEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(m => m.CardNumber, null as string)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.NullOrEmpty");
            _validator.ShouldHaveValidationErrorFor(m => m.CardNumber, string.Empty)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.NullOrEmpty");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenCardNumberLengthIsLessThan9()
        {
            _validator.ShouldHaveValidationErrorFor(m => m.CardNumber, "00124")
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.ExactLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenCardNumberLengthIsMoreThan9()
        {
            _validator.ShouldHaveValidationErrorFor(m => m.CardNumber, "1240588012741")
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.ExactLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenSurnameGivenNameAndDateOfBirthAreNullOrEmpty()
        {
            var model = new NationalIdVerificationRequest
            {
               Nin = "CM0588012VTHUE",
               CardNumber = "426463810",
            };

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("Surname", result.Errors.Select(x => x.PropertyName));
            Assert.Contains("NationalIdVerificationRequest.OptionalFields.NullOrEmpty", result.Errors.Select(x => x.ErrorCode));
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenDatabaseCredentialsAreUsedButNotSet()
        {
            _niraSettingsMock.Setup(x => x.Value)
                .Returns(new NiraSettings
                {
                    CredentialConfig = new CredentialConfig { UseDatabaseCredentials = true}
                });

            _credentialServiceMock.Setup(x => x.AreDatabaseCredentialsSet()).ReturnsAsync(false);

            _validator.ShouldHaveValidationErrorFor(x => x.Nin, "CF200721001NRA")
                .WithErrorCode("NationalIdVerificationRequest.Base.DatabaseCredentialsHaveNotBeenSet");
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenConfigCredentialsAreNotSet()
        {
            _credentialServiceMock.Setup(x => x.AreConfigCredentialsSet()).Returns(false);

            _validator.ShouldHaveValidationErrorFor(x => x.Nin, "CF200721001NRA")
                .WithErrorCode("NationalIdVerificationRequest.Base.ConfigCredentialsHaveNotBeenSet");
        }
        
        [Fact]
        public void Validator_ShouldHaveError_WhenConfigCredentialsAreExpired()
        {
            var model = new NationalIdVerificationRequest
            {
                Nin = "CM0588012VTHUE",
                CardNumber = "426463810",
                Surname = "Lawson"
            };

            _credentialServiceMock.Setup(x => x.GetCurrentCredentialsAsync())
                .ReturnsAsync(new DTOs.Credentials.CredentialResponse
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now.AddDays(-79),
                    ExpiresOn = DateTime.Now.AddDays(-20),
                    Username = "Emata@BankROOT"
                });

            var result = _validator.Validate(model);

            Assert.False(result.IsValid);
            Assert.Contains("NationalIdVerificationRequest.Base.ExpiredCredentials", result.Errors.Select(x => x.ErrorCode));
            Assert.Contains("NIRA credentials are expired. Please contact your system administrator.", result.Errors.Select(x => x.ErrorMessage));
        }


        [Fact]
        public void Validator_ShouldHaveError_WhenNitaCredentialsAreUsedButNotSet()
        {
            _verificationOptionsMock.Setup(x => x.Value)
                .Returns(new VerificationSettings
                {
                    ConnectionType = ConnectionType.Nita
                });

            _nitaCredentialServiceMock.Setup(x => x.AreNitaCredentialsSet()).ReturnsAsync(false);

            _validator.ShouldHaveValidationErrorFor(x => x.Nin, "CF200721001NRA")
                .WithErrorCode("NationalIdVerificationRequest.Base.NitaCredentialsHaveNotBeenSet")
                .WithErrorMessage("NITA client credentials have not been set. Please contact your system administrator.");
        }
    }
}
