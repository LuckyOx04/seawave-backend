namespace Core.Models;

public class Track
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
}