namespace Core.DTOs;

public record UserProfileResponse(string Username, string Email, DateTime CreatedAt, int CreatedPlaylistsCount,
    int PendingTracksCount, int ApprovedTracksCount);