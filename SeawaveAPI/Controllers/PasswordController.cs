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
    
    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request) 
        => Ok(await passwordService.ResetPasswordAsync(request.Token, request.NewPassword));

    [HttpPatch("change")]
    [SessionAuthorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        int userId = (int)HttpContext.Items["UserId"]!;
        
        return Ok(await passwordService.ChangePasswordAsync(userId, request.CurrentPassword,
            request.NewPassword, request.ConfirmPassword));
    }
}