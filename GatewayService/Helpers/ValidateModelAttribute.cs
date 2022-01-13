using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GatewayService.Helpers
{
#nullable disable
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) 
                return;
            var errorsList = context.ModelState.Select(it =>
            {
                return (Field: it.Key, Errors: it.Value.Errors.Select(er => er.ErrorMessage).ToList());
            }).ToList();

            var errorDict = new Dictionary<string, List<string>>();
            foreach (var (field, errors) in errorsList)
            {
                errorDict[LowerInvariant(field)] = errors;
            }

            var error = new
            {
                Message = "The request is invalid",
                Errors = errorDict
            };
            context.Result = new BadRequestObjectResult(error);
        }

        private static string LowerInvariant(string word)
        {
            return char.ToLowerInvariant(word[0]) + word.Substring(1);
        }
    }

    public class ValidGuidAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' does not contain a valid guid";

        public ValidGuidAttribute() : base(DefaultErrorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (!Guid.TryParse(input, out var guid))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return guid == Guid.Empty ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName)) : null;
        }
    }

    public class MustHaveElementsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is ICollection list)
            {
                return list.Count > 0;
            }
            return false;
        }
    }
}