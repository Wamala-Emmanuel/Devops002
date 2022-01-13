using System.Collections.Generic;
using GatewayService.Services.Nira;

namespace GatewayService.DTOs
{
#nullable disable
    public class SearchResponse
    {
        public SearchPagination Pagination { get; set; }
        public List<RequestViewModel> Requests { get; set; } 

    }

    /// <summary>
    /// NIRA verify card details response
    /// </summary>
    public class VerificationResult
    {
        /// <summary>
        /// Whether the card details provided match those of NIRA
        /// </summary>
        public bool MatchingStatus { get; set; }

        /// <summary>
        /// The card status according to NIRA
        /// </summary>
        public string CardStatus { get; set; }

        /// <summary>
        /// The status from NIRA
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// The days left for the NIRA password to expire
        /// </summary>
        public int? PasswordDaysLeft { get; set; }


        /// <summary>
        /// The cost of making a request to NIRA 
        /// </summary>
        public double ExecutionCost { get; set; }
        
        /// <summary>
        /// The error details when the status from NIRA is error
        /// </summary>
        public ResponseError Error { get; set; }

        /// <summary>
        /// Whether the status from NIRA is error
        /// </summary>
        public bool IsError { get; set; }
    }

    public class TransactionStatus
    {
        /// <summary>
        /// The status from NIRA
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The days left for the NIRA password to expire
        /// </summary>
        public int PasswordDaysLeft { get; set; }

        /// <summary>
        /// The cost of making a request to NIRA 
        /// </summary>
        public double ExecutionCost { get; set; }
    }
}
