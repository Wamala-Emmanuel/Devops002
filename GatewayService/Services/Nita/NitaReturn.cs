using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatewayService.Services.Nita
{
    public class NitaReturn
    {
        [JsonProperty("transactionStatus")]
        public TransactionStatus TransactionStatus { get; set; }

        [JsonProperty("matchingStatus")]
        public bool MatchingStatus { get; set; }

        [JsonProperty("cardStatus")]
        public string CardStatus { get; set; }

    }
}
