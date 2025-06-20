using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Integration
{
    [Produces("application/json")]
    [Route("api/integracao")]
    [ApiController]
    public class IntegrationController : Controller
    {
        private readonly IIntegrationService _integrationService;
        private readonly IConfiguration _configuration;

        public IntegrationController(IIntegrationService integrationService,
            IConfiguration configuration)
        {
            _integrationService = integrationService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addorupdate")]
        public async Task<IActionResult> AddOrUpdateIntegration([FromBody] HubIntegracaoDto integration)
        {
            try
            {

                if (integration == null)
                    return BadRequest(new Response<HubIntegracaoDto> { Error = new ErrorResult("Object is null") });

                var result = await _integrationService.AddOrUpdateIntegrationAsync(integration);

                if (!result.IsSuccess)
                    return new BadRequestObjectResult(result);

                // Never return sensitive authentication data to the client
                // Tokens are cleared before sending the response
                result.Result.Password = string.Empty;
                result.Result.RefreshToken = string.Empty;
                result.Result.Token = string.Empty;

                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                return BadRequest(new Response<HubIntegracaoDto> { Error = new ErrorResult(e.Message) });
            }
        }

    }
}