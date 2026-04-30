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
    
    public async Task AddTrackToPlaylistAsync(int playlistId, int trackId)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_AddTrackToPlaylist",
            new { p_playlist_id = playlistId, p_track_id = trackId },
            commandType: CommandType.StoredProcedure);
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

    public async Task<IEnumerable<Playlist>> SearchPlaylistsAsync(string query)
    {
        using var connection = db.CreateConnection();
        return await connection.QueryAsync<Playlist>("sp_SearchPlaylists",
            new { p_query = query },
            commandType: CommandType.StoredProcedure);
    }

    public async Task RequestUploadAsync(int userId, string title, string artist, string tempPath)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_RequestUpload",
            new { p_user_id = userId, p_title = title, p_artist = artist, p_temp_path = tempPath },
            commandType: CommandType.StoredProcedure);
    }
}