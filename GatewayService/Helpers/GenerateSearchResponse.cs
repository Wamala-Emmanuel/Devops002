using System;
using System.Collections.Generic;
using Bogus;
using GatewayService.DTOs;
using GatewayService.Models;
using Newtonsoft.Json;

namespace GatewayService.Helpers
{
#nullable disable
    public class GenerateSearchResponse
    {
        /// <summary>
        /// Generates a list of search request
        /// </summary>
        /// <param name="request"></param>
        public static (List<RequestViewModel>, int totalItems) GetSampleResults(SearchRequest request)
        {
            var resultOptions = new[] { "Valid", "Invalid", "Pending" };
            var statusOptions = new[] { "Ok", "Failed" };
            var totalItems = request.Id != null ? 1 : new Faker().Random.Number(request.Pagination?.ItemsPerPage ?? 50, 1000);
            
            var transactionStatus = new Faker<TransactionStatus>()
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000,decimals: 1)));
            
            var niraResult = new Faker<VerificationResultViewModel>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                //.RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                //.RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                ;

            var requestDetails = new Faker<RequestViewModel>()
                .RuleFor(r => r.CardNumber, f => request.CardNumber ?? f.Random.Int(min: 100000000, max: 999999999).ToString())
                .RuleFor(r => r.DateOfBirth, f => f.Date.Past(18, new DateTime(2002, 1, 1, 0,0,0)).Date)
                .RuleFor(r => r.Surname, f => request.Surname ?? f.Name.FirstName())
                .RuleFor(r => r.GivenNames, f => request.GivenNames ?? f.Name.LastName())
                .RuleFor(n => n.Nin, f => request.Nin ?? $"C{f.PickRandom<Gender>()}{StringExtensions.GetRandomString(StringExtensions.randomSequence, 12)}")
                .RuleFor(r => r.RequestStatus, f => f.PickRandom<RequestStatus>()) 
                .RuleFor(r => r.Initiator, f => request.Initiator ?? f.Name.FirstName())
                .RuleFor(r => r.ResultJson, f => niraResult.Generate())
                .RuleFor(r => r.Id, f => request.Id ?? f.Random.Guid())
                .RuleFor(r => r.ParticipantId, f => f.Random.Guid())
                .RuleFor(r => r.ReceivedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.ReceivedFromNira, f => f.Date.Past().ToUniversalTime())
                .RuleFor(r => r.SubmittedAt, f => f.Date.Past().ToUniversalTime());

            return request.Id != null ? (requestDetails.Generate(1), totalItems) : (requestDetails.Generate(request.Pagination?.ItemsPerPage ?? 50), totalItems);
        }
    }

    public enum Gender
    {
        M,
        F
    }

    public class NiraCardDetails
    {
        /// <summary>
        /// NIN request card details
        /// </summary>
        public string Name { get; set; }
        public string Nin { get; set; }
        public string CardNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
