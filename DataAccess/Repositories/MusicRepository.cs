using System.Data;
using Core.Models;
using Dapper;

namespace DataAccess.Repositories;

public class MusicRepository(IDbConnectionFactory db)
{
    public async Task<IEnumerable<Track>> SearchTracksAsync(string query)
    {
        using var connection = db.CreateConnection();
        return await connection.QueryAsync<Track>("sp_SearchTracks",
            new { p_query = query },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreatePlaylistAsync(string name, int userId)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_CreatePlaylist",
            new { p_name = name, p_user_id = userId },
            commandType: CommandType.StoredProcedure);
    }
    
    public async Task<bool> AddTrackToPlaylistAsync(int userId, int playlistId, int trackId)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_AddTrackToPlaylist",
            new { p_user_id = userId, p_playlist_id = playlistId, p_track_id = trackId },
            commandType: CommandType.StoredProcedure) == 1;
    }

    public async Task<int> RemoveTrackFromPlaylistAsync(int userId, int playlistId, int trackId)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_RemoveTrackFromPlaylist",
            new { p_user_id = userId, p_playlist_id = playlistId, p_track_id = trackId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeletePlaylistAsync(int userId, int playlistId)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_DeletePlaylist",
            new { p_user_id = userId, p_playlist_id = playlistId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PlaylistSummary>> SearchPlaylistsAsync(string query)
    {
        using var connection = db.CreateConnection();
        return await connection.QueryAsync<PlaylistSummary>("sp_SearchPlaylists",
            new { p_query = query },
            commandType: CommandType.StoredProcedure);
    }

    private async Task<PlaylistSummary?> GetPlaylistByIdAsync(int playlistId)
    {
        using var connection = db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PlaylistSummary>("sp_GetPlaylistById",
            new { p_playlist_id = playlistId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PlaylistSummary>> GetPlaylistsByUserIdAsync(int userId)
    {
        using var connection = db.CreateConnection();
        return await connection.QueryAsync<PlaylistSummary>("sp_GetPlaylistByUserId",
            new { p_user_id = userId },
            commandType: CommandType.StoredProcedure);
    }
    
    public async Task<PlaylistDetails?> GetPlaylistDetailsAsync(int playlistId)
    {
        var playlistSummary = await GetPlaylistByIdAsync(playlistId);

        if (playlistSummary == null)
        {
            return null;
        }
        
        using var connection = db.CreateConnection();
        var tracks = await connection.QueryAsync<Track>("sp_GetPlaylistTracks",
            new { p_playlist_id = playlistId },
            commandType: CommandType.StoredProcedure);

        return new PlaylistDetails(
            playlistSummary.Id,
            playlistSummary.Name,
            playlistSummary.CreatorId,
            playlistSummary.CreatorName,
            tracks.ToList());
    }

    public async Task RequestUploadAsync(int userId, string title, string artist, string tempFileName)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_RequestUpload",
            new { p_user_id = userId, p_title = title, p_artist = artist, p_temp_file_name = tempFileName },
            commandType: CommandType.StoredProcedure);
    }
}