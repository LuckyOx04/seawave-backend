namespace Core.Models;

public class PlaylistDetails(int id, string name, int creatorId, string creatorName, List<Track>? tracks)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int CreatorId { get; set; } = creatorId;
    public string CreatorName { get; set; } = creatorName;
    public List<Track>? Tracks { get; set; } = tracks;
}