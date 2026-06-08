namespace Core.DTOs;

public record UserProfileResponse(
    string Username,
    string Email,
    DateTimeOffset CreatedAt,
    long CreatedPlaylistsCount,
    long PendingTracksCount,
    long ApprovedTracksCount)
{
    public UserProfileResponse(
        string username,
        string email,
        DateTime createdAt,
        long createdPlaylistsCount,
        long pendingTracksCount,
        long approvedTracksCount)
        : this(username, email, ConvertMariaDbTimestamp(createdAt), createdPlaylistsCount, pendingTracksCount,
            approvedTracksCount)
    {
    }

    private static DateTimeOffset ConvertMariaDbTimestamp(DateTime timestamp)
    {
        if (timestamp.Kind == DateTimeKind.Unspecified || timestamp.Kind == DateTimeKind.Utc)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(timestamp, DateTimeKind.Utc));
        }

        return new DateTimeOffset(timestamp);
    }
}