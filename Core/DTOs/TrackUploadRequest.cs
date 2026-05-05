using Microsoft.AspNetCore.Http;

namespace Core.DTOs;

public class TrackUploadRequest
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public IFormFile File { get; set; } = null!;
}