using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.DTOs.NitaCredentials;
using GatewayService.Helpers;
using GatewayService.Services.Nita.NitaCredentialService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GatewayService.Controllers
{
    [Route("api/nitacredentials")]
    [ApiVersion("1.0")]
    [SwaggerTag("Endpoints to manage client credentials from NITA.")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NitaCredentialsController : BaseController
    {
        private readonly INitaCredentialService _service;
        private readonly ILogger<NitaCredentialsController> _logger;
        public NitaCredentialsController(INitaCredentialService service,
            ILogger<NitaCredentialsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        ///   Add client credentials from NITA
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>
        /// Accepts client credentials of the participant to make requests to NITA.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(
            statusCode: StatusCodes.Status201Created,
            description: "The NITA client credentials have been set.",
            typeof(NitaCredentialResponse))]
        public async Task<ActionResult<NitaCredentialResponse>> PostAsync(
            [FromBody, SwaggerRequestBody("The NITA client credentials payload", Required = true)] NitaCredentialRequest request)
        {
            var response = await _service.SetNitaCredentialsAsync(request);

            _logger.LogInformation("NITA Client Credentials for {clientKey} have been set.", request.ClientKey);

            return CreatedAtAction(nameof(GetCurrent), response);
        }

        /// <summary>
        /// Returns the current NITA clinet credentials that are set in the application.
        /// The NIRA Client Credentials set in the database will return with an Id and CreatedOn value.
        /// Otherwise the Id and CreatedOn value will not be set 
        /// </summary>
        /// <returns></returns>
        [HttpGet("current")]
        [SwaggerResponse(
            statusCode: StatusCodes.Status200OK,
            description: "The current NIRA Credentials",
            typeof(NitaCredentialResponse))]
        public async Task<ActionResult<NitaCredentialResponse>> GetCurrent()
        {
            try
            {
                var response = await _service.GetCurrentNitaCredentialsAsync();

                _logger.LogInformation("The current NITA Client Credentials for {clientKey} have been found.", response.ClientKey);
                return Ok(response);
            }
            catch (NotFoundException)
            {
                _logger.LogInformation("NITA Client Credentials have not been set.");
                return NotFound("NITA Client Credentials have not been set.");
            }
        }

        /// <summary>
        /// Edit tiered rates for a specified service category
        /// </summary>
        /// <param name="id"> Id of NITA client credentials to be updated</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(
            statusCode: StatusCodes.Status200OK,
            description: "The updated NIRA Credentials",
            typeof(NitaCredentialResponse))]
        public async Task<ActionResult<NitaCredentialResponse>> EditAsync(
            [FromRoute] Guid id, [FromBody] NitaCredentialRequest model)
        {
            var response = await _service.UpdateNitaCredentialsAsync(id, model);
            return Ok(response);
        }
    }
}
