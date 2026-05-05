namespace Core.DTOs;

public record LoginResponse(bool Success, string Message, string? SessionToken);