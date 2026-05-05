namespace Core.DTOs;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);