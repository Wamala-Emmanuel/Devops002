using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Services.Nira;
using Newtonsoft.Json;

namespace GatewayService.Services.Nita
{
    public class TransactionStatus
    {
        [JsonProperty("transactionStatus")]
        public string TransactionStatusTransactionStatus { get; set; }

        [JsonProperty("passwordDaysLeft")]
        public int PasswordDaysLeft { get; set; }

        public double ExecutionCost { get; set; }

        [JsonProperty("error")]
        public NitaError Error { get; set; }

        public bool IsError { get; set; }
    }
}
