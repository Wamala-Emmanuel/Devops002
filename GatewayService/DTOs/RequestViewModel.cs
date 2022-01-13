using System;
using System.ComponentModel.DataAnnotations;
using GatewayService.Models;
using GatewayService.Services.Nira;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GatewayService.DTOs
{
#nullable disable
    /// <summary>
    /// The details of the request
    /// </summary>
    public class RequestViewModel
    {
        /// <summary>
        /// The unique id for the request
        /// </summary>
        public Guid Id { get; set; }

#nullable enable
        /// <summary>
        /// The names for the request initiator
        /// </summary>
        public string? Initiator { get; set; }

        /// <summary>
        /// The unique id for the request initiator
        /// </summary>
        public Guid? InitiatorId { get; set; }

        /// <summary>
        /// The email for user that initiated the request
        /// </summary>
        public string? InitiatorEmail { get; set; }

        /// <summary>
        /// The unique id for the participant that initiated the request
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
        /// Date the response was received from NIRA
        /// </summary>
        public DateTime? ReceivedFromNira { get; set; }

#nullable disable

        /// <summary>
        /// Status of the request
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestStatus RequestStatus { get; set; }

#nullable enable

        /// <summary>
        /// Surname on the card that was verified
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// Given names on the card that was verified
        /// </summary>f
        public string? GivenNames { get; set; }

#nullable disable

        /// <summary>
        /// ID card number on the card that was verified
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Masked ID Card number on the card that was verified
        /// </summary>
        public string MaskedCardNumber { get; set; }

        /// <summary>
        /// National Identification Number on the card that was verified
        /// </summary>
        public string Nin { get; set; }

        /// <summary>
        /// Masked National Identification Number on the card that was verified
        /// </summary>
        public string MaskedNin { get; set; }

#nullable enable
        /// <summary>
        /// Date of birth on the card that was verified
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }


        public VerificationResultViewModel? ResultJson { get; set; }

#nullable disable
    }

    /// <summary>
    /// The response from NIRA based on the card details provided
    /// </summary>
    public class VerificationResultViewModel
    {
        /// <summary>
        /// Whether the card details matched those at NIRA
        /// </summary>
        public bool? MatchingStatus { get; set; }

        /// <summary>
        /// Whether the card is valid or not.
        /// </summary>
        public string CardStatus { get; set; }

        /// <summary>
        /// The NIN request status as returned from NIRA
        /// </summary>
        public string Status { get; set; }


        public ResponseError Error { get; set; }

        /// <summary>
        /// When true, it means that NIRA found an error with the request.
        /// See <seealso cref="ResponseError"/> for more information.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// The Nin status field for the request
        /// </summary>
        public string NinStatus
        {
            get
            {
                if (IsError && Error != null) return Error.Message;

                return Status;
            }
        }
    }
}
