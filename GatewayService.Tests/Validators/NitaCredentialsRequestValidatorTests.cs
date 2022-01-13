using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using GatewayService.DTOs.NitaCredentials;
using GatewayService.Validators;
using Xunit;

namespace GatewayService.Tests.Validators
{
    public class NitaCredentialsRequestValidatorTests
    {
        private readonly NitaCredentialsRequestValidator _validator;

        public NitaCredentialsRequestValidatorTests()
        {
            _validator = new NitaCredentialsRequestValidator();
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenClientSecretIsNullOrEmpty()
        {
            var request = new NitaCredentialRequest
            {
                ClientSecret = string.Empty,
                ClientKey = "abc"
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.ClientSecret)
                .WithErrorMessage("'Client Secret' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NitaCredentialRequest.ClientSecret.NullOrEmpty");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenClientKeyIsNullOrEmpty()
        {
            var request = new NitaCredentialRequest
            {
                ClientSecret = "sdartxvcx45fsasfk==",
                ClientKey = string.Empty
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.ClientKey)
                .WithErrorMessage("'Client Key' must not be empty.")
                .WithSeverity(Severity.Error)
                .WithErrorCode("NitaCredentialRequest.ClientKey.NullOrEmpty");
        }

    }
}
