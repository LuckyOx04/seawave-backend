using DataAccess.Repositories;
using Org.BouncyCastle.Security;
using Services.Helpers;
using BC = BCrypt.Net.BCrypt;

namespace Services;

public class PasswordService(UserRepository userRepository, EmailService emailService)
{
    public async Task RequestPasswordResetAsync(string email)
    {
        var token = Guid.NewGuid().ToString();
        bool exists = await userRepository.SetPasswordResetTokenAsync(email, token, 30);

        if (exists)
        {
            await emailService.SendEmailAsync(email, "Password Reset", 
                $"<p>Your reset token is: <b>{token}</b></p>");
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        if (!Validator.IsValidPassword(newPassword))
        {
            throw new FormatException("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }

        var hash = BC.HashPassword(newPassword);
        return await userRepository.ResetPasswordWithTokenAsync(token, hash);
    }

    public async Task<bool> ChangePasswordAsync(string identifier, string currentPass, string newPass, string confirmPass)
    {
        if (newPass != confirmPass)
        {
            throw new FormatException("Confirmed password does not match the new password.");
        }
        if (!Validator.IsValidPassword(newPass))
        {
            throw new FormatException("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }

        var user = await userRepository.GetByLoginAsync(identifier);
        if (user == null)
        {
            return false;
        }

        if (!BC.Verify(currentPass, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Wrong current password.");
        }

        var newHash = BC.HashPassword(newPass);
        return await userRepository.ChangePasswordAsync(user.Id, newHash) > 0;
    }
}