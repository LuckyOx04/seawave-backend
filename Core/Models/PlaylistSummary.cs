namespace Core.Models;

public class PlaylistSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public int TrackCount { get; set; }
}