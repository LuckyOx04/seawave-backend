using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using SeawaveAPI.Attributes;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(MusicService musicService) : ControllerBase
{
    [HttpGet("search/tracks")]
    public async Task<IActionResult> SearchTracks([FromQuery] string q)
    {
        var result = await musicService.SearchTrackAsync(q);
        return result.Any() ? Ok(result) : NotFound();
    }

    [HttpGet("search/playlists")]
    public async Task<IActionResult> SearchPlaylists([FromQuery] string q)
    {
        var result = await musicService.SearchPlaylistAsync(q);
        return result.Any() ? Ok(result) : NotFound();
    }

    [HttpGet("playlist/{playlistId:int}")]
    public async Task<IActionResult> GetPlaylistDetails([FromRoute] int playlistId)
    {
        var playlistDetails = await musicService.GetPlaylistDetailsAsync(playlistId);
        return playlistDetails != null ? Ok(playlistDetails) : NotFound();
    }

    [HttpGet("stream/{fileName}")]
    public async Task<IActionResult> StreamTrack([FromRoute] string fileName)
    {
        var stream = await musicService.GetTrackStreamAsync(fileName);
        return File(stream, "audio/mpeg", enableRangeProcessing: true);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [SessionAuthorize]
    public async Task<IActionResult> UploadTrack([FromForm] TrackUploadRequest request)
    {
        var userId = (int)HttpContext.Items["UserId"]!;
        
        await using var stream = request.File.OpenReadStream();
        await musicService.RequestUploadAsync(userId, request.Title, request.Artist, request.File.FileName, stream);
        return Accepted();
    }
}