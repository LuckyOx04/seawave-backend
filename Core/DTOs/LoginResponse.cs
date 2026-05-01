namespace Core.DTOs;

public record LoginResponse(bool Success, string Message, string? SessionToken, 
    string? Username, bool IsConfirmed = true);