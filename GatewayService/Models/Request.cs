using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GatewayService.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GatewayService.Models
{
#nullable disable
    public class Request
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public Guid Id { get; set; }

#nullable enable
        /// <summary>
        /// The name the request initiator
        /// </summary>
        public string? Initiator { get; set; }

        /// <summary>
        /// The userid for the request initiator
        /// </summary>
        public Guid? InitiatorId { get; set; }

        /// <summary>
        /// The email for user that initiated the request
        /// </summary>
        public string? InitiatorEmail { get; set; }

        /// <summary>
        /// The Participant that initiated the request
        /// </summary>
        public Guid? ParticipantId { get; set; }
#nullable disable

        /// <summary>
        /// Date the request was created
        /// </summary>
        public DateTime ReceivedAt { get; set; }

#nullable enable
        /// <summary>
        /// Date the request was submitted to NIRA
        /// </summary>
        public DateTime? SubmittedAt { get; set; }

        /// <summary>
        /// Date the request was received from NIRA
        /// </summary>
        public DateTime? ReceivedFromNira { get; set; }

        /// <summary>
        /// Date billing was updated with the request info
        /// </summary>
        public DateTime? BillingUpdated { get; set; }
#nullable disable

        /// <summary>
        /// Status of the request
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus RequestStatus { get; set; }

#nullable enable
        /// <summary>
        /// Result of the ID verification from NIRA
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// Surname on the ID
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// Given names on the ID
        /// </summary>
        public string? GivenNames { get; set; }

#nullable disable

        /// <summary>
        /// ID Card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// National Identification Number
        /// </summary>
        public string Nin { get; set; }

#nullable enable
        /// <summary>
        /// Date of birth
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
#nullable disable

        [NotMapped]
        public string Name
        {
            get
            {
                var name = string.Empty;
                if (!string.IsNullOrEmpty(Surname)) name = Surname;

                if (!string.IsNullOrEmpty(GivenNames)) name += $" {GivenNames}";

                return name;
            }
        }

        [NotMapped]
        public VerificationResult VerificationResult => Result != null
            ? JsonConvert.DeserializeObject<VerificationResult>(Result,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore})
            : null;

    }
}
