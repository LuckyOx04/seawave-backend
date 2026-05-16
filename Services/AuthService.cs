using Core.DTOs;
using DataAccess.Repositories;
using Services.Helpers;
using BC = BCrypt.Net.BCrypt;

namespace Services;

public class AuthService(UserRepository userRepository, SessionRepository sessionRepository, EmailService emailService)
{
    public async Task<string> RegisterAsync(RegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || !Validator.IsValidUsername(request.Username))
        {
            throw new FormatException("Username cannot be empty or contain @ symbol.");
        }
        if (!Validator.IsValidEmail(request.Email))
        {
            throw new FormatException("Invalid email format.");
        }
        if (!Validator.IsValidPassword(request.Password))
        {
            throw new FormatException("Password must have at least 8 characters," +
                                "an upper case letter, a lower case letter and a digit.");
        }
        if (request.Password != request.ConfirmPassword)
        {
            throw new FormatException("Passwords do not match.");
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

        var emailBody = await HtmlFormattingService.GetVerifyEmailBody(token);
        await emailService.SendEmailAsync(request.Email, "Confirm your account", emailBody);

        return "Registration successful. Please, check your email.";
    }

    public async Task<bool> ConfirmEmailAsync(string token) => await userRepository.ConfirmEmailAsync(token);

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var identifier = Validator.IsValidUsername(request.Identifier) ?
            request.Identifier.Slugify() : request.Identifier;
        var user = await userRepository.GetByLoginAsync(identifier);

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

        return new LoginResponse(user.Username, user.Email, token);
    }

    public async Task<int?> ValidateSessionAsync(string token) => await sessionRepository.ValidateSessionAsync(token);

    public async Task LogoutAsync(string token) => await sessionRepository.DeleteSessionAsync(token);

    public async Task<UserProfileResponse?> GetUserProfileInfoAsync(int userId) => 
        await userRepository.GetUserProfileInfoAsync(userId);
}