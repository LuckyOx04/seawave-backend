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
    
    [HttpPost]
    public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request) 
        => Ok(await musicService.CreatePlaylistAsync(request.Name, CurrentUserId));

    [HttpDelete("{playlistId:int}")]
    public async Task<IActionResult> DeletePlaylist([FromRoute] int playlistId)
    {
        var success = await musicService.DeletePlaylistAsync(CurrentUserId, playlistId);
        return success ? NoContent() : Forbid();
    }

    [HttpPost("{playlistId:int}/tracks/{trackId:int}")]
    public async Task<IActionResult> AddTrack([FromRoute] int playlistId, [FromRoute] int trackId)
    {
        var success = await musicService.AddTrackToPlaylistAsync(CurrentUserId, playlistId, trackId);
        return success ? Created() : Forbid();
    }

    [HttpDelete("{playlistId:int}/tracks/{trackId:int}")]
    public async Task<IActionResult> RemoveTrack([FromRoute] int playlistId, [FromRoute] int trackId)
    {
        var success = await musicService.RemoveTrackFromPlaylistAsync(CurrentUserId, playlistId, trackId);
        return success ? NoContent() : Forbid();
    }
}