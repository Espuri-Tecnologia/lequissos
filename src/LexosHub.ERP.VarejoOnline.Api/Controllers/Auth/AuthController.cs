
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Auth;

[Produces("application/json")]
[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly IVarejoOnlineApiService _varejoOnlineApiService;
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration,
        IVarejoOnlineApiService varejoOnlineApiService)
    {
        _configuration = configuration;
        _varejoOnlineApiService = varejoOnlineApiService;
    }
    [HttpPost]
    [Route("authUrl")]
    public async Task<IActionResult> GetOAuthUrl()
    {
        try
        {
            var result = await _varejoOnlineApiService.GetAuthUrl();

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

        var tokenResponse = await _varejoOnlineApiService.ExchangeCodeForTokenAsync(code);

        return Ok("Token salvo com sucesso!");
    }
}