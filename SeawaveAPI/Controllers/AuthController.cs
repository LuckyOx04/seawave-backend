using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationRequest req) => Ok(await authService.RegisterAsync(req));
    
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token) => Ok(await authService.ConfirmEmailAsync(token));
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req) => Ok(await authService.LoginAsync(req));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
    {
        await authService.LogoutAsync(token);
        return NoContent();
    }
}