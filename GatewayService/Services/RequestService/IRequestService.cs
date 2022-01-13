using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;
using Microsoft.AspNetCore.Http;

namespace GatewayService.Services
{
    public interface IRequestService
    {
        Task<Guid> Process(NationalIdVerificationRequest request, HttpRequest httpRequest);
    }
}
