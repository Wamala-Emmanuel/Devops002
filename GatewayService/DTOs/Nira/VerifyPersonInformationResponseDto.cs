using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GatewayService.DTOs.Nira
{
#nullable disable
    public class VerifyPersonInformationResponseDto
    {
        public bool MatchingStatus { get; set; }
        public string CardStatus { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }

    public class TransactionStatus
    {

    }
}
