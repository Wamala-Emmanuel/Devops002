using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.Helpers;
using GatewayService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GatewayService.Controllers
{
    [Route("api/exports")]
    [ApiVersion("1.0")]
    [SwaggerTag("A selection of previous requests can be exported to CSV format. Please see below the end points to do this.")]
    public class ExportsController : BaseController
    {
        private readonly IExportService _service;
        private readonly ILogger<ExportService> _logger;

        public ExportsController(IExportService service, ILogger<ExportService> logger)
        {
            _logger = logger;
            _service = service;
        }


        /// <summary>
        /// Get Export Status
        /// </summary>
        /// <remarks>Endpoint for getting export status for a specific export request by unique id.</remarks>
        /// <param name="id"></param>
        /// <example>8754b7cb-d0fc-4499-8a1a-ebfb721cf0fc</example>
        /// <returns></returns>
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "The request exists with a given status",
            typeof(ExportStatusResponse))]
        [HttpGet("{id:Guid}/status", Name = nameof(GetRequestStatus))]
        public async Task<IActionResult> GetRequestStatus([FromRoute] Guid id)
        {
            var apiVersion = HttpContext.GetRequestedApiVersion() ?? new ApiVersion(1, 0);

            var result = await _service.CheckRequestStatusAsync(id, apiVersion);

            return Ok(result);
        }


        /// <summary>
        /// Download Exported Requests
        /// </summary>
        /// <remarks>Endpoint for downloading an export of requests based on an id.</remarks>
        /// <param name="id">id</param>
        /// <example>8754b7cb-d0fc-4499-8a1a-ebfb721cf0fc</example>
        /// <returns></returns>
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Download the export",
            typeof(FileContentResult))]
        [HttpGet("{id:Guid}/download", Name = "download")]
        public async Task<IActionResult> DownloadAsync([FromRoute]Guid id)
        {
            var file = await _service.DownloadRequestsExportAsync(id);

            return File(file.Contents, file.ContentType, file.Name);
        }


        /// <summary>
        /// Make an export of verification requests
        /// </summary>
        /// <remarks>
        /// Accepts request for export verification requests. Request is queued and processed as soon as possible.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(
            statusCode: StatusCodes.Status202Accepted,
            description: "The request has been received and accepted for processing.",
            typeof(ExportStatusResponse))]
        [ValidateModel]
        public async Task<IActionResult> Create(
            [FromBody, SwaggerRequestBody("The export request payload", Required = true)] ExportRequest request)
        {
            var apiVersion = HttpContext.GetRequestedApiVersion();
            
            var response = await _service.ExportAsync(request, HttpContext.Request);
            return AcceptedAtAction(nameof(GetRequestStatus), 
                value: response,
                routeValues: new {id = response.Id});
        }
    }
}