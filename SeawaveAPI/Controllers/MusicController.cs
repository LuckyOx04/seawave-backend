using Microsoft.AspNetCore.Mvc;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(MusicService musicService) : ControllerBase
{
    [HttpGet("search/tracks")]
    public async Task<IActionResult> SearchTracks(string query) => Ok(await musicService.SearchTrackAsync(query));
    
    [HttpGet("search/playlists")]
    public async Task<IActionResult> SearchPlaylists(string query) => Ok(await musicService.SearchPlaylistAsync(query));

    [HttpGet("stream/{fileName}")]
    public async Task<IActionResult> StreamTrack(string fileName)
    {
        var stream = await musicService.GetTrackStreamAsync(fileName);
        return File(stream, "audio/mpeg", enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTrack(int userId, string title, string artist, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        await musicService.RequestUploadAsync(userId, title, artist, file.FileName, stream);
        return Ok();
    }
}