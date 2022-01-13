using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;

namespace GatewayService.Services.BillingService
{
    public interface IBillingService
    {
        Task CreateNotificationAsync(BillingRequest request);
        Task UpdateBilling(Guid requestId);
    }
}
