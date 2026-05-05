using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using SeawaveAPI.Attributes;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest req) 
        => Ok(await authService.RegisterAsync(req));

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        var success = await authService.ConfirmEmailAsync(token);
        return success ? Ok() : BadRequest();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req) => Ok(await authService.LoginAsync(req));

    [HttpPost("logout")]
    [SessionAuthorize]
    public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
    {
        await authService.LogoutAsync(token);
        return NoContent();
    }
}