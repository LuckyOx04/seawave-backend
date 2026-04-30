namespace Core.DTOs;

public record ChangePasswordRequest(string OldPassword, string NewPassword);