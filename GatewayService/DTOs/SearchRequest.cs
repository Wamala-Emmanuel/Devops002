using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GatewayService.Models;

namespace GatewayService.DTOs
{
    public class SearchRequest
    {
#nullable enable
        /// <summary>
        /// Search for a request using their unique Id
        /// </summary>
        public Guid? Id { get; set; }

#nullable disable
        /// <summary>
        /// Date range when the request(s) were sent for verification
        /// </summary>
        public DateRange Date { get; set; }

#nullable enable
        /// <summary>
        /// Search using National Id Number on the card
        /// </summary>
        [MinLength(14)]
        [MaxLength(14)]
        public string? Nin { get; set; }

        /// <summary>
        /// Search using surname on the card
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// Search using given names on the card
        /// </summary>
        public string? GivenNames { get; set; }

        /// <summary>
        /// Search using the matching status of the request
        /// </summary>
        public bool? MatchingStatus { get; set; }

        /// <summary>
        /// Search using the status of the request
        /// </summary>
        public RequestStatus? Status { get; set; }

        /// <summary>
        /// Search using the card number on the card
        /// </summary>
        [MinLength(9)]
        [MaxLength(9)]
        public string? CardNumber { get; set; }

        /// <summary>
        /// Search using the user that initiated the request
        /// </summary>
        public string? Initiator { get; set; }


        public SearchPagination Pagination { get; set; }

        public SearchRequest()
        {
            Pagination = new SearchPagination
            {
                ItemsPerPage = 50,
                Page = 1
            };
        }
    }

    /// <summary>
    /// Search for request based on a date range
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// The start date of the date range
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// The end date of the date range
        /// </summary>
        public DateTime? To { get; set; }
    }
}
