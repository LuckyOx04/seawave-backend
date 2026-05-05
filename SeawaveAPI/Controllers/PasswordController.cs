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

    [HttpGet("reset-page")]
    public IActionResult GetResetPage([FromQuery] string token)
    {
        return Content($$"""
        <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body { font-family: sans-serif; display: flex; align-items: center; justify-content: center; height: 100vh; margin: 0; background-color: #f4f7f9; }
                    .card { background: white; padding: 30px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.1); width: 100%; max-width: 400px; }
                    input { width: 100%; padding: 12px; margin: 10px 0; border: 1px solid #ddd; border-radius: 4px; box-sizing: border-box; }
                    button { width: 100%; padding: 12px; background-color: #007BFF; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; }
                    button:disabled { background-color: #ccc; }
                    .error { color: #d9534f; font-size: 13px; display: none; margin-bottom: 10px; }
                </style>
            </head>
            <body>
                <div class='card'>
                    <h2 style='margin-top: 0;'>New Password</h2>
                    <form id='resetForm' action='/api/password/reset-action' method='POST'>
                        <input type='hidden' name='token' value='{{token}}' />
                        <input type='password' id='newPassword' name='newPassword' placeholder='New Password' required />
                        <input type='password' id='confirmPassword' placeholder='Confirm New Password' required />
                        <div id='errorMsg' class='error'>Passwords must match and be at least 8 characters with uppercase, lowercase, and a number.</div>
                        <button type='submit' id='submitBtn'>Update Password</button>
                    </form>
                </div>
                <script>
                    const form = document.getElementById('resetForm');
                    const pass = document.getElementById('newPassword');
                    const confirm = document.getElementById('confirmPassword');
                    const error = document.getElementById('errorMsg');
                    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$/;
            
                    form.onsubmit = function(e) {
                        if (!regex.test(pass.value) || pass.value !== confirm.value) {
                            e.preventDefault();
                            error.style.display = 'block';
                            return false;
                        }
                    };
                </script>
            </body>
        </html>
        """, "text/html");
    }

    [HttpPost("reset-action")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> ResetPassword([FromForm] string token, [FromForm] string newPassword, 
        [FromForm] string confirmPassword)
    {
        var success = await passwordService.ResetPasswordAsync(token, newPassword, 
            confirmPassword);
        
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