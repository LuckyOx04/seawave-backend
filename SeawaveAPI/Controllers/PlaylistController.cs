using Microsoft.AspNetCore.Mvc;
using SeawaveAPI.Attributes;
using Services;

namespace SeawaveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistController(MusicService musicService) : ControllerBase
{
    private int CurrentUserId => (int)HttpContext.Items["UserId"]!;
    
    [HttpPost]
    [SessionAuthorize]
    public async Task<IActionResult> CreatePlaylist(string name, int userId) => 
        Ok(await musicService.CreatePlaylistAsync(name, userId));

    [HttpDelete("{playlistId}")]
    [SessionAuthorize]
    public async Task<IActionResult> DeletePlaylist(int userId, int playlistId) =>
        Ok(await musicService.DeletePlaylistAsync(userId, playlistId));

    [HttpPost("{playlistId}/tracks/{trackId}")]
    [SessionAuthorize]
    public async Task<IActionResult> AddTrack(int playlistId, int trackId)
    {
        await musicService.AddTrackToPlaylistAsync(playlistId, trackId);
        return Created();
    }
    
    [HttpDelete("{playlistId}/tracks/{trackId}")]
    [SessionAuthorize]
    public async Task<IActionResult> RemoveTrack(int userId, int playlistId, int trackId) =>
        Ok(await musicService.RemoveTrackFromPlaylistAsync(userId, playlistId, trackId));
}