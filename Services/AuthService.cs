using Core.DTOs;
using DataAccess.Repositories;
using Services.Helpers;
using BC = BCrypt.Net.BCrypt;

namespace Services;

public class AuthService(UserRepository userRepository, SessionRepository sessionRepository, EmailService emailService)
{
    public async Task<string> RegisterAsync(RegistrationRequest request)
    {
        if (!Validator.IsValidEmail(request.Email))
        {
            throw new FormatException("Invalid email format.");
        }
        if (!Validator.IsValidPassword(request.Password))
        {
            throw new FormatException("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new FormatException("Username cannot be empty.");
        }

        var slug = request.Username.Slugify();

        if (userRepository.GetByLoginAsync(slug).Result != null)
        {
            throw new UnauthorizedAccessException("Username already taken.");
        }
        if (userRepository.GetByLoginAsync(request.Email).Result != null)
        {
            throw new UnauthorizedAccessException("Email already taken.");
        }

        var hash = BC.HashPassword(request.Password);
        var userId = await userRepository.RegisterAsync(slug, request.Email, hash);
        
        var token = Guid.NewGuid().ToString();
        await userRepository.SetEmailVerificationTokenAsync(userId, token);

        await emailService.SendEmailAsync(request.Email, "Confirm your account", 
            $"""
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee;'>
                    <h2 style='color: #2D3E50;'>Welcome to Seawave!</h2>
                    <p>Please click the button below to verify your email address and activate your account.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://localhost:7212/api/auth/confirm-email?token={token}'
                            style='background-color: #007BFF; color: white; padding: 14px 25px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Verify Email Address
                        </a>
                    </div>
                    <p style='font-size: 0.8em; color: #666;'>If you didn't create an account, you can safely ignore this email.</p>
                </div>
                """);

        return "Registration successful. Please, check your email.";
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        return await userRepository.ConfirmEmailAsync(token);
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByLoginAsync(request.Identifier);

        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        if (!user.IsConfirmed)
        {
            throw new UnauthorizedAccessException("Email not confirmed.");
        }

        var token = Guid.NewGuid().ToString();
        await sessionRepository.CreateSessionAsync(token, user.Id, 7);

        return token;
    }

    public async Task<int?> ValidateSessionAsync(string token)
    {
        return await sessionRepository.ValidateSessionAsync(token);
    }

    public async Task LogoutAsync(string token)
    {
        await sessionRepository.DeleteSessionAsync(token);
    }
}