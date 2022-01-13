using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Models;
using Newtonsoft.Json;

namespace GatewayService.DTOs
{
    public class ExportRequest
    {
        [Required]
        public DateFilter DateRange { get; set; }


#nullable enable

        /// <summary>
        /// Export based on the National Id Number in the request
        /// </summary>
        [MinLength(14)]
        [MaxLength(14)]
        public string? Nin { get; set; }

        /// <summary>
        /// Export based on Id Card number in the request
        /// </summary>
        [MinLength(9)]
        [MaxLength(9)]
        public string? CardNumber { get; set; }

        /// <summary>
        /// Export based on the status of the request
        /// </summary>
        public ICollection<string>? RequestStatus { get; set; }

        /// <summary>
        /// Export requests based on the validity of the NIN number
        /// </summary>
        public string? NinValidity { get; set; }

        /// <summary>
        /// Export based on the matching status of the request
        /// </summary>
        public bool? MatchingStatus { get; set; }

#nullable disable

    }

    /// <summary>
    /// Date range used to specify which verification requests to export
    /// </summary>
    public class DateFilter
    {
        /// <summary>
        /// start date of date range
        /// </summary>
        [Required]
        public DateTime From { get; set; }

        [JsonIgnore]
        public DateTime FromDate => From.Date;

        /// <summary>
        /// End date of date range
        /// </summary>
        [Required]
        public DateTime To { get; set; }

        [JsonIgnore]
        public DateTime ToDate => To.Date.AddDays(1).AddMilliseconds(-1);
    }

}
