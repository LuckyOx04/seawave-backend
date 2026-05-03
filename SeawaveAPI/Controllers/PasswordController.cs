using Microsoft.AspNetCore.Mvc;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordController(PasswordService passwordService) : ControllerBase
{
    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        await passwordService.RequestPasswordResetAsync(email);
        return Ok(new { message = "A reset link was sent to the email." });
    }
    
    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword(string token, string newPassword) => 
        Ok(await passwordService.ResetPasswordAsync(token, newPassword));
    
    [HttpPatch("change")]
    public async Task<IActionResult> ChangePassword(string identifier, string currentPassword, string newPassword,
        string confirmPassword) => Ok(await passwordService.ChangePasswordAsync(identifier, currentPassword,
        newPassword, confirmPassword));
}