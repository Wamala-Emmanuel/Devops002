using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _validator.ShouldHaveValidationErrorFor(c => c.Password, new CredentialRequest
                {
                    Password = string.Empty,
                    Username = "abc"
                })
                .WithErrorCode("CredentialRequest.Password.NullOrEmpty");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoDigit()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "CmDavf")
                .WithErrorCode("CredentialRequest.Password.Digit");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoLowerCaseLetter()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "CD1234")
                .WithErrorCode("CredentialRequest.Password.LowerCaseLetter");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoUpperCaseLetter()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "ab1234")
                .WithErrorCode("CredentialRequest.Password.UpperCaseLetter");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasNoSpecialCharacterLetter()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "aB1234")
                .WithErrorCode("CredentialRequest.Password.SpecialCharacter");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordHasWhiteSpace()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "aB12 !4")
                .WithErrorCode("CredentialRequest.Password.WhiteSpace");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordLengthIsLessThan6()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "CmD1*")
                .WithErrorCode("CredentialRequest.Password.MinMaxLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenPasswordLengthIsGreaterThan10()
        {
            _validator.ShouldHaveValidationErrorFor(n => n.Password, "CmD12856478|5")
                .WithErrorCode("CredentialRequest.Password.MinMaxLengthValidator");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenUsernameIsNullOrEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(c => c.Username, new CredentialRequest
                {
                    Username = string.Empty, Password = "abc24!F"
                })
                .WithErrorCode("CredentialRequest.Username.NullOrEmpty");
        }
    }
}
