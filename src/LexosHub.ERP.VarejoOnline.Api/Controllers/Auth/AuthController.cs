
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Auth;

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

        return Ok("Token salvo com sucesso!");
    }
}