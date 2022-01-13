using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs;
using GatewayService.DTOs.Credentials;
using GatewayService.Helpers;
using GatewayService.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GatewayService.Controllers
{
    [Route("api/credentials")]
    [ApiVersion("1.0")]
    [SwaggerTag("Endpoints to manage credentials from Nira.")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CredentialsController : BaseController
    {
        private readonly ICredentialService _service;
        private readonly ILogger<CredentialService> _logger;
        private readonly IBackgroundJobClient _backgroundJob;

        public CredentialsController(ICredentialService service, ILogger<CredentialService> logger, IBackgroundJobClient backgroundJob)
        {
            _logger = logger;
            _service = service;
            _backgroundJob = backgroundJob;

        }

        /// <summary>
        /// Add credentials from Nira
        /// </summary>
        /// <remarks>
        /// Accepts credentials of the participant to make requests to nira.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(
            statusCode: StatusCodes.Status201Created,
            description: "The credentials have been set.",
            typeof(CredentialResponse))]
        [ValidateModel] 
        public async Task<ActionResult<CredentialResponse>> SetCredentials(
            [FromBody, SwaggerRequestBody("The NIRA credentials payload", Required = true)] CredentialRequest request)
        {
            var response = await _service.SetCredentialsAsync(request);
            
            _logger.LogInformation("Credentials for {niraUsername} have been set.", request.Username);

            return CreatedAtAction( nameof(GetCurrent), response);

        }
        
        /// <summary>
        /// Returns the current NIRA credentials that are set in the application.
        /// The NIRA Credentials set in the database will return with an Id and ExpiresOn value.
        /// Otherwise the Id and ExpiresOn value will not be set if the application is configured to use credentials set in application
        /// configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("current")]
        [SwaggerResponse(
            statusCode:StatusCodes.Status200OK, 
            description:"The current NIRA Credentials",
            typeof(CredentialResponse))]
        public async Task<ActionResult<CredentialResponse>> GetCurrent()
        {
            try
            {
                var response = await _service.GetCurrentCredentialsAsync();

                _logger.LogInformation("The current NIRA Credentials for {niraUsername} have been found.", response.Username);
                return Ok(response);
            }
            catch (NotFoundException)
            {
                return NotFound($"NIRA Credentials have not been set.");
            }


        }
    }
}
