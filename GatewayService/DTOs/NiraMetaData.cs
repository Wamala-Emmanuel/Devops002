using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayService.DTOs
{
    public class NiraMetaData
    {
        /// <summary>
        ///  Whether the information provided is the same as that at NIRA
        /// </summary>
        public bool IsMatching { get; set; }

#nullable enable
        /// <summary>
        /// The current status of the national ID
        /// </summary>
        public string? IsCardValid { get; set; }
#nullable disable

        /// <summary>
        /// Status of the response from NIRA (Either Ok or Error)
        /// </summary>
        public string NiraResponseStatus { get; set; }

        /// <summary>
        /// ID Card number
        /// </summary>
        public string CardNumber { get; set; }

#nullable enable
        /// <summary>
        /// The user that initiated the request
        /// </summary>
        public string? Initiator { get; set; }

        /// <summary>
        /// Date the request was submitted to NIRA
        /// </summary>
        public DateTime? SubmittedAt { get; set; }

        /// <summary>
        /// Date the request was received from NIRA
        /// </summary>
        public DateTime? ReceivedFromNira { get; set; }

#nullable disable
        /// <summary>
        /// National Identification Number
        /// </summary>
        public string Nin { get; set; }

        /// <summary>
        /// Gender of the person whose details are being a verified 
        /// </summary>
        public string Gender { get; set; }

#nullable enable
        /// <summary>
        /// Date of birth
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
#nullable disable
    }
}
