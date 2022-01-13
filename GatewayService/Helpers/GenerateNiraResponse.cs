using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using GatewayService.DTOs;

namespace GatewayService.Helpers
{
    public class GenerateNiraResponse
    {
        public static VerificationResult GetSampleResult()
        {
            var resultOptions = new[] { "Valid", "Invalid", "Pending" };
            var statusOptions = new[] { "Ok", "Failed" };

            var transactionStatus = new Faker<TransactionStatus>()
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                .Generate();

            var niraResult = new Faker<VerificationResult>()
                .RuleFor(n => n.MatchingStatus, f => f.Random.Bool())
                .RuleFor(n => n.CardStatus, f => f.Random.ArrayElement(resultOptions))
                .RuleFor(n => n.Status, f => f.Random.ArrayElement(statusOptions))
                .RuleFor(n => n.PasswordDaysLeft, f => f.Random.Number(min: 0, max: 180))
                .RuleFor(n => n.ExecutionCost, f => Convert.ToDouble(f.Commerce.Price(min: 0, max: 2000, decimals: 1)))
                .Generate();

            return niraResult;
        }
    }
}
