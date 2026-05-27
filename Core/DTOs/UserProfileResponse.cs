namespace Core.DTOs;

public record UserProfileResponse(string Username, string Email, DateTime CreatedAt, long CreatedPlaylistsCount,
    long PendingTracksCount, long ApprovedTracksCount);