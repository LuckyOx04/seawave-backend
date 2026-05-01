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
            throw new Exception("Invalid email format.");
        }
        if (!Validator.IsValidPassword(request.Password))
        {
            throw new Exception("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }

        var hash = BC.HashPassword(request.Password);
        await userRepository.RegisterAsync(request.Username, request.Email, hash);

        var user = await userRepository.GetByLoginAsync(request.Email);
        var token = Guid.NewGuid().ToString();
        await userRepository.SetEmailVerificationTokenAsync(user!.Id, token);

        await emailService.SendEmailAsync(user.Email, "Confirm your account",
            $"<h1>Welcome!</h1><p>Use this token to verify: <b>{token}</b></p>");

        return "Please, check your email.";
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        return await userRepository.ConfirmEmailAsync(token);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByLoginAsync(request.LoginIdentifier);

        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResponse(false, "Invalid credentials", null, null);
        }
        if (!user.IsConfirmed)
        {
            return new LoginResponse(false, "Email not confirmed", null, user.Username, false);
        }

        var token = Guid.NewGuid().ToString();
        await sessionRepository.CreateSessionAsync(token, user.Id, 7);

        return new LoginResponse(true, "Success", token, user.Username);
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