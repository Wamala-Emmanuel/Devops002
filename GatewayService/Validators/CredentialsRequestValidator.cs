using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Repositories.Contracts;

namespace GatewayService.Validators
{
    public class CredentialsRequestValidator : AbstractValidator<CredentialRequest>
    {
        public CredentialsRequestValidator()
        {
            RuleFor(c => c.Username)
                .NotEmpty()
                .WithErrorCode("CredentialRequest.Username.NullOrEmpty");
            
            RuleFor(c => c.Password)
                .NotEmpty()
                .WithErrorCode("CredentialRequest.Password.NullOrEmpty")
                .Matches("[0-9]")
                .WithMessage($"The password should include a digit (0-9) at least once.")
                .WithErrorCode("CredentialRequest.Password.Digit")
                .Matches("[a-z]")
                .WithMessage($"The password should include a lower case letter at least once.")
                .WithErrorCode("CredentialRequest.Password.LowerCaseLetter")
                .Matches("[A-Z]")
                .WithMessage($"The password should include an upper case letter at least once.")
                .WithErrorCode("CredentialRequest.Password.UpperCaseLetter")
                .Matches(@"[-!$%^&*()_+|~=`{}\[\]:\/;<>?,.@#]")
                .WithMessage($"The password should include a special character at least once.")
                .WithErrorCode("CredentialRequest.Password.SpecialCharacter")
                .Must(p => !p.Any(c => char.IsWhiteSpace(c)))
                .WithMessage($"The password should have no white-space.")
                .WithErrorCode("CredentialRequest.Password.WhiteSpace")
                .Length(6, 10)
                .WithMessage($"The password should have at least 6 characters, maximum 10")
                .WithErrorCode("CredentialRequest.Password.MinMaxLengthValidator");

        }
    }
}
