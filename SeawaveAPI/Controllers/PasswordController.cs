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
        return Ok(new { message = "A reset link was sent to the email." });
    }

    [HttpGet("reset-page")]
    public async Task<IActionResult> GetResetPage([FromQuery] string token)
    {
        var page = await HtmlFormattingService.GetPasswordChangeFormPage(token);
        
        return Content(page, "text/html");
    }

    [HttpPost("reset-action")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> ResetPassword([FromForm] string token, [FromForm] string newPassword, 
        [FromForm] string confirmPassword)
    {
        var success = await passwordService.ResetPasswordAsync(token, newPassword, 
            confirmPassword);

        var webPage = await HtmlFormattingService.GetPasswordChangeResultPage(success);

        return Content(webPage, "text/html");
    }

    [HttpPatch("change")]
    [SessionAuthorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        
        await passwordService.ChangePasswordAsync(userId, request.CurrentPassword,
            request.NewPassword, request.ConfirmPassword);
        return NoContent();
    }
}