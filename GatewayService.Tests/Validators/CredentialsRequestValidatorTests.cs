using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Validators;
using Moq;
using Xunit;

namespace GatewayService.Tests.Validators
{
    public class CredentialsRequestValidatorTests
    {
        private readonly CredentialsRequestValidator _validator;
        
        public CredentialsRequestValidatorTests()
        {
            _validator = new CredentialsRequestValidator();

        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordIsNullOrEmpty()
        {
            var request = new CredentialRequest
            {
                Password = string.Empty,
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("'Password' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.NullOrEmpty");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoDigit()
        {
            var request = new CredentialRequest
            {
                Password = "CmDavf",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should include a digit (0-9) at least once.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.Digit");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoLowerCaseLetter()
        {
            var request = new CredentialRequest
            {
                Password = "CD1234",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should include a lower case letter at least once.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.LowerCaseLetter");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoUpperCaseLetter()
        {
            var request = new CredentialRequest
            {
                Password = "ab1234",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should include an upper case letter at least once.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.UpperCaseLetter");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoSpecialCharacterLetter()
        {
            var request = new CredentialRequest
            {
                Password = "aB1234",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should include a special character at least once.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.SpecialCharacter");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasWhiteSpace()
        {
            var request = new CredentialRequest
            {
                Password = "aB12 !4",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should have no white-space.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.WhiteSpace");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordLengthIsLessThan6()
        {
            var request = new CredentialRequest
            {
                Password = "CmD1*",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should have at least 6 characters, maximum 10")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.MinMaxLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordLengthIsGreaterThan10()
        {
            var request = new CredentialRequest
            {
                Password = "CmD12856478|5",
                Username = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("The password should have at least 6 characters, maximum 10")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Password.MinMaxLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenUsernameIsNullOrEmpty()
        {
            var request = new CredentialRequest
            {
                Password = "abc24!F",
                Username = string.Empty
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage("'Username' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("CredentialRequest.Username.NullOrEmpty");
        }
    }
}
