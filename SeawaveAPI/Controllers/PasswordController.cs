using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using SeawaveAPI.Attributes;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordController(PasswordService passwordService) : ControllerBase
{
    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgottenPasswordRequest request)
    {
        await passwordService.RequestPasswordResetAsync(request.Email);
        return Ok("A reset link was sent to the email.");
    }

    [HttpPost("reset-action")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var success = await passwordService.ResetPasswordAsync(request.Token, request.NewPassword, 
            request.ConfirmPassword);
        
        var title = success ? "Password changed." : "Password reset failed.";
        var message = success
            ? "Your password has been updated. You can now log in to the Seawave app with your new credentials."
            : "The link has expired or the token is invalid.";
        var color = success ? "#28a745" : "#dc3545";

        return Content($"""
            <html>
                <body style='font-family: sans-serif; display: flex; align-items: center; justify-content: center; height: 100vh; margin: 0; background-color: #f9f9f9;'>
                    <div style='text-align: center; padding: 50px; background: white; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); max-width: 400px;'>
                        <h1 style='color: {color};'>{title}</h1>
                        <p style='color: #555;'>{message}</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                        <p style='color: #888; font-size: 0.9em;'>You can close this tab and return to the application.</p>
                    </div>
                </body>
            </html>
            """, "text/html");
    }

    [HttpPatch("change")]
    [SessionAuthorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        
        var success = await passwordService.ChangePasswordAsync(userId, request.CurrentPassword,
            request.NewPassword, request.ConfirmPassword);
        return success ? Ok() : Forbid();
    }
}