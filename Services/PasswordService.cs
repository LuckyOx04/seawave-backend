using DataAccess.Repositories;
using Services.Helpers;
using BC = BCrypt.Net.BCrypt;

namespace Services;

public class PasswordService(UserRepository userRepository, EmailService emailService)
{
    public async Task RequestPasswordResetAsync(string email)
    {
        var token = Guid.NewGuid().ToString();
        var exists = await userRepository.SetPasswordResetTokenAsync(email, token, 30);

        if (exists)
        {
            await emailService.SendEmailAsync(email, "Password Reset", 
            $"""
            <html>
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee;'>
                    <h2 style='color: #2D3E50;'>Password Reset Request</h2>
                    <p>We received a request to reset your password. Click the button below to choose a new one.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://localhost:7212/api/password/reset-page?token={token}' 
                           style='background-color: #007BFF; color: white; padding: 14px 25px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                           Reset Password
                        </a>
                    </div>
                    <p style="font-size: 0.8em; color: #666">If you didn't request this, you can ignore this email. The link will expire shortly.</p>
                </div>
            </html>
            """);
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, string confirmPassword)
    {
        if (!Validator.IsValidPassword(newPassword))
        {
            throw new FormatException("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }
        if (newPassword != confirmPassword)
        {
            throw new FormatException("Confirmed password does not match the new password.");
        }

        var hash = BC.HashPassword(newPassword);
        return await userRepository.ResetPasswordWithTokenAsync(token, hash);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPass, string newPass, string confirmPass)
    {
        if (!Validator.IsValidPassword(newPass))
        {
            throw new FormatException("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }
        if (newPass != confirmPass)
        {
            throw new FormatException("Confirmed password does not match the new password.");
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new FileNotFoundException("User not found.");
        }

        if (!BC.Verify(currentPass, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Wrong current password.");
        }

        var newHash = BC.HashPassword(newPass);
        return await userRepository.ChangePasswordAsync(user.Id, newHash) > 0;
    }
}