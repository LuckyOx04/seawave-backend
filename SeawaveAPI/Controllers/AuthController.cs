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
        => Ok(new { message = await authService.RegisterAsync(req) });

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        var success = await authService.ConfirmEmailAsync(token);
        var title = success ? "Email confirmed" : "Link Expired";
        var message = success ? "Your email has been confirmed. You can now log in to the desktop app." 
            : "This confirmation link is invalid or has already been used.";
        var color = success ? "#28a745" : "#F44336";
        return Content(
            $"""
            <html>
                <body style='font-family: sans-serif; display: flex; align-items: center; justify-content: center; height: 100vh; margin: 0; background-color: #f9f9f9;'>
                    <div style='text-align: center; padding: 50px; background: white; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>
                        <h1 style='color: {color};'>{title}</h1>
                        <p style='color: #555; font-size: 1.1em;'>{message}</p>
                        <p style='margin-top: 20px; color: #888;'>You may close this window.</p>
                    </div>
                </body>
            </html>
            """, "text/html");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req) => Ok(new { token = await authService.LoginAsync(req) });

    [HttpPost("logout")]
    [SessionAuthorize]
    public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string token)
    {
        await authService.LogoutAsync(token);
        return NoContent();
    }
}