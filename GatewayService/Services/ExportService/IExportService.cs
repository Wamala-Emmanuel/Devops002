using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Services
{
    public interface IExportService
    {
        Task<ExportStatusResponse> CheckRequestStatusAsync(Guid id, ApiVersion apiVersion);

        Task<FileViewModel> DownloadRequestsExportAsync(Guid id);

        Task<ExportStatusResponse> ExportAsync(ExportRequest request, HttpRequest httpRequest);
    }
}
