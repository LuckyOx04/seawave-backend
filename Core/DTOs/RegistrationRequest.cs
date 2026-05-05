namespace Core.DTOs;

public record RegistrationRequest(string Username, string Email, string Password, string ConfirmPassword);