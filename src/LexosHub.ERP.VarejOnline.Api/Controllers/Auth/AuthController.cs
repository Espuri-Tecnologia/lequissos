
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejOnline.Api.Controllers.Auth;

[Produces("application/json")]
[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration,
        IAuthService authService)
    {
        _configuration = configuration;
        _authService = authService;
    }
    [HttpPost]
    [Route("authUrl")]
    public async Task<IActionResult> GetOAuthUrl()
    {
        try
        {
            var result = await _authService.GetAuthUrl();

            return new OkObjectResult(result);
        }
        catch (Exception e)
        {
            return BadRequest(new Response<HubIntegracaoDto> { Error = new ErrorResult(e.Message) });
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Código não informado.");

        var tokenResponse = await _authService.EnableTokenIntegrationAsync(code);

        return Ok(tokenResponse);
    }
}