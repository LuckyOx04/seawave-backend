using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using SeawaveAPI.Attributes;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[SessionAuthorize]
public class PlaylistController(MusicService musicService) : ControllerBase
{
    private int CurrentUserId => (int)HttpContext.Items["UserId"]!;

    [HttpGet]
    public async Task<IActionResult> GetPlaylistsForUser([FromRoute] int userId)
        => Ok(await musicService.GetPlaylistsByUserIdAsync(CurrentUserId));

    [HttpPost("create")]
    public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request)
    {
        var createdPlaylistId = await musicService.CreatePlaylistAsync(request.Name, CurrentUserId);
        return Created(new Uri($"api/Music/playlist/{createdPlaylistId}"), new
        {
            playlistId = createdPlaylistId,
            playlistName = request.Name
        });
    }

    [HttpDelete("delete/{playlistId:int}")]
    public async Task<IActionResult> DeletePlaylist([FromRoute] int playlistId)
    {
        var success = await musicService.DeletePlaylistAsync(CurrentUserId, playlistId);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("{playlistId:int}/add-track/{trackId:int}")]
    public async Task<IActionResult> AddTrack([FromRoute] int playlistId, [FromRoute] int trackId)
    {
        var success = await musicService.AddTrackToPlaylistAsync(CurrentUserId, playlistId, trackId);
        if (!success)
        {
            return NotFound();
        }
        
        var playlistDetails = await musicService.GetPlaylistDetailsAsync(playlistId);
        return Ok(playlistDetails);
    }

    [HttpDelete("{playlistId:int}/remove-track/{trackId:int}")]
    public async Task<IActionResult> RemoveTrack([FromRoute] int playlistId, [FromRoute] int trackId)
    {
        var success = await musicService.RemoveTrackFromPlaylistAsync(CurrentUserId, playlistId, trackId);
        if (!success)
        {
            return NotFound();
        }
     
        var playlistDetails = await musicService.GetPlaylistDetailsAsync(playlistId);
        return Ok(playlistDetails);
    }
}