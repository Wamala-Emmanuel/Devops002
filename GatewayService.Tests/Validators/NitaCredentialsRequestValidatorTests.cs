using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _validator.ShouldHaveValidationErrorFor(c => c.ClientSecret, new NitaCredentialRequest
            {
                ClientSecret = string.Empty,
                ClientKey = "abc"
            })
            .WithErrorCode("NitaCredentialRequest.ClientSecret.NullOrEmpty");
        }

        [Fact]
        public void Validator_ShouldHaveErrorWhenClientKeyIsNullOrEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(c => c.ClientKey, new NitaCredentialRequest
            {
                ClientSecret = "sdartxvcx45fsasfk==",
                ClientKey = string.Empty
            })
            .WithErrorCode("NitaCredentialRequest.ClientKey.NullOrEmpty");
        }

    }
}
