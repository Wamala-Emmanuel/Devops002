using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Helpers.Nira;
using GatewayService.Models;
using Newtonsoft.Json;

namespace GatewayService.Helpers.Mappers
{
    public static class MapperProfiles
    {
        public static RequestViewModel MapRequestModelToRequestViewModel(Request request)
        {
            var resultJson = JsonConvert.DeserializeObject<VerificationResultViewModel>(request.Result ?? "{}",
                         new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            if (resultJson.CardStatus == null)
            {
                resultJson.CardStatus = "Error";
            }

            return new RequestViewModel
            {
                Id = request.Id,
                Initiator = request.Initiator ?? null,
                InitiatorId = request.InitiatorId ?? null,
                InitiatorEmail = request.InitiatorEmail ?? null,
                ParticipantId = request.ParticipantId ?? null,
                ReceivedAt = request.ReceivedAt,
                SubmittedAt = request.SubmittedAt ?? null,
                ReceivedFromNira = request.ReceivedFromNira ?? null,
                RequestStatus = request.RequestStatus,
                ResultJson = resultJson,
                Surname = request.Surname,
                GivenNames = request.GivenNames,
                CardNumber = request.CardNumber,
                MaskedCardNumber = NiraUtils.MaskCardDetails(request.CardNumber, charsToShow: 5),
                Nin = request.Nin,
                MaskedNin = NiraUtils.MaskCardDetails(request.Nin, charsToShow: 6),
                DateOfBirth = request.DateOfBirth ?? null
            }; 
        }
    }
}
