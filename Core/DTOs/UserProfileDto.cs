namespace Core.DTOs;

public record UserProfileDto(string Username, string Email, DateTime CreatedAt, int CreatedPlaylistsCount,
    int PendingTracksCount, int ApprovedTracksCount);