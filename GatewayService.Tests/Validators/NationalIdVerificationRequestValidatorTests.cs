using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
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
        public void Validator_Should_HaveAnErrorWhenNinIsNull()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = null,
                DateOfBirth = DateTime.UtcNow,
                CardNumber = "000092896",
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Nin)
                .WithErrorMessage("'Nin' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.Nin.NullOrEmpty");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenNinIsEmpty()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = string.Empty,
                DateOfBirth = DateTime.UtcNow,
                CardNumber = "000092896",
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Nin)
                .WithErrorMessage("'Nin' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.Nin.NullOrEmpty");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenNinLengthIsLessThan14()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CFJKL0MNKLW",
                DateOfBirth = DateTime.UtcNow,
                CardNumber = "000092896",
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Nin)
                .WithErrorMessage("'Nin' must be 14 characters in length. You entered 11 characters.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.Nin.ExactLengthValidator");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenNinLengthIsGreaterThan14()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CFJKL0MNKLW4562RTKLN",
                DateOfBirth = DateTime.UtcNow,
                CardNumber = "000092896",
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Nin)
                .WithErrorMessage("'Nin' must be 14 characters in length. You entered 20 characters.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.Nin.ExactLengthValidator");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenCardNumberIsNull()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CF640761001DDD",
                DateOfBirth = DateTime.UtcNow,
                CardNumber = null,
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CardNumber)
                .WithErrorMessage("'Card Number' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.NullOrEmpty");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenCardNumberIsEmpty()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CF640761001DDD",
                DateOfBirth = DateTime.UtcNow,
                CardNumber = string.Empty,
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CardNumber)
                .WithErrorMessage("'Card Number' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.NullOrEmpty");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenCardNumberLengthIsLessThan9()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CF640761001DDD",
                DateOfBirth = DateTime.UtcNow,
                CardNumber = "00009289",
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CardNumber)
                .WithErrorMessage("'Card Number' must be 9 characters in length. You entered 8 characters.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.ExactLengthValidator");
        }

        [Fact]
        public void Validator_Should_HaveAnErrorWhenCardNumberLengthIsGreaterThan9()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CF640761001DDD",
                DateOfBirth = DateTime.UtcNow,
                CardNumber = "0000928961234",
                GivenNames = "SUSAN",
                Surname = "ADONG",
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CardNumber)
                .WithErrorMessage("'Card Number' must be 9 characters in length. You entered 13 characters.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.CardNumber.ExactLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenSurnameGivenNameAndDateOfBirthAreNullOrEmpty()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CF640761001DDD",
                CardNumber = "000092896"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Surname)
                .WithErrorMessage("Include atleast given names or the date of birth")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.OptionalFields.NullOrEmpty");

            result.ShouldHaveValidationErrorFor(x => x.GivenNames)
                .WithErrorMessage("Include atleast a surname or the date of birth")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.OptionalFields.NullOrEmpty");

            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
                .WithErrorMessage("Include atleast a surname or given names")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NationalIdVerificationRequest.OptionalFields.NullOrEmpty");
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenNitaCredentialsAreUsedButNotSet()
        {
            var request = new NationalIdVerificationRequest
            {
                Nin = "CF640761001DDD",
                CardNumber = "000092896"
            };

            _verificationOptionsMock.Setup(x => x.Value)
                .Returns(new VerificationSettings
                {
                    ConnectionType = ConnectionType.Nita
                });

            _nitaCredentialServiceMock.Setup(x => x.AreNitaCredentialsSet()).ReturnsAsync(false);

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Nin)
                .WithErrorCode("NationalIdVerificationRequest.Base.NitaCredentialsHaveNotBeenSet")
                .WithSeverity(Severity.Error)
                .WithErrorMessage("NITA client credentials have not been set. Please contact your system administrator.");
        }
    }
}
