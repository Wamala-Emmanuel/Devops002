using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace GatewayService.DTOs
{
    public class NationalIdVerificationRequest
    {
#nullable enable
        /// <summary>
        /// Surname on the card
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// Given names on the card
        /// </summary>
        public string? GivenNames { get; set; }

        /// <summary>
        /// Date of birth on the card
        /// </summary>
        [SwaggerSchema(Format = "date")]
        public DateTime? DateOfBirth { get; set; }

#nullable disable

        /// <summary>
        /// National ID Number on the card
        /// </summary>
        [Required]
        [MinLength(14)]
        [MaxLength(14)]
        public string Nin { get; set; }

        /// <summary>
        /// Card number of the card
        /// </summary>
        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string CardNumber { get; set; }
    }
}
