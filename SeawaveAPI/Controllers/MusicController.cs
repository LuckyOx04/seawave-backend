using Microsoft.AspNetCore.Mvc;
using SeawaveAPI.Attributes;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(MusicService musicService) : ControllerBase
{
    [HttpGet("search/tracks")]
    public async Task<IActionResult> SearchTracks(string q) => Ok(await musicService.SearchTrackAsync(q));
    
    [HttpGet("search/playlists")]
    public async Task<IActionResult> SearchPlaylists(string q) => Ok(await musicService.SearchPlaylistAsync(q));

    [HttpGet("stream/{fileName}")]
    public async Task<IActionResult> StreamTrack(string fileName)
    {
        var stream = await musicService.GetTrackStreamAsync(fileName);
        return File(stream, "audio/mpeg", enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    [SessionAuthorize]
    public async Task<IActionResult> UploadTrack(int userId, string title, string artist, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        await musicService.RequestUploadAsync(userId, title, artist, file.FileName, stream);
        return Ok();
    }
}