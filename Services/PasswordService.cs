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
            var emailBody = await HtmlFormattingService.GetPasswordResetEmailBody(token);
            await emailService.SendEmailAsync(email, "Password Reset", emailBody);
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

    public async Task ChangePasswordAsync(int userId, string currentPass, string newPass, string confirmPass)
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
        await userRepository.ChangePasswordAsync(user.Id, newHash);
    }
}