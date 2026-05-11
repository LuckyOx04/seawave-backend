using Core.Models;
using DataAccess.Repositories;

namespace Services;

public class MusicService
{
    private readonly MusicRepository _musicRepository;
    private readonly string _basePath;

    public MusicService(MusicRepository musicRepository)
    {
        _musicRepository = musicRepository;
        _basePath = Path.Combine(AppContext.BaseDirectory, "MusicStorage");

        Directory.CreateDirectory(Path.Combine(_basePath, "Library"));
        Directory.CreateDirectory(Path.Combine(_basePath, "Pending"));
    }
    
    public async Task<IEnumerable<Track>> SearchTrackAsync(string query) 
        => await _musicRepository.SearchTracksAsync(query);
    
    public async Task<IEnumerable<PlaylistSummary>> SearchPlaylistAsync(string query) 
        => await _musicRepository.SearchPlaylistsAsync(query);

    public async Task<PlaylistDetails?> GetPlaylistDetailsAsync(int playlistId)
        => await _musicRepository.GetPlaylistDetailsAsync(playlistId);

    public async Task<IEnumerable<PlaylistSummary>> GetPlaylistsByUserIdAsync(int userId)
        => await _musicRepository.GetPlaylistsByUserIdAsync(userId);


    public async Task<int> CreatePlaylistAsync(string name, int userId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new FormatException("Playlist name is required.");
        }
        
        return await _musicRepository.CreatePlaylistAsync(name, userId);
    }

    public async Task<bool> DeletePlaylistAsync(int userId, int playlistId) 
        => await _musicRepository.DeletePlaylistAsync(userId, playlistId) > 0;

    public async Task<bool> AddTrackToPlaylistAsync(int userId, int playlistId, int trackId)
        => await _musicRepository.AddTrackToPlaylistAsync(userId, playlistId, trackId);

    public async Task<bool> RemoveTrackFromPlaylistAsync(int userId, int playlistId, int trackId) 
        => await _musicRepository.RemoveTrackFromPlaylistAsync(userId, playlistId, trackId) > 0;

    public Task<FileStream> GetTrackStreamAsync(string fileName)
    {
        var path = Path.Combine(_basePath, "Library", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File not found.");
        }

        return Task.FromResult(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, true));
    }

    public async Task RequestUploadAsync(int userId, string title, string artist, string fileName, Stream fileStream)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(artist))
        {
            throw new FormatException("Title and Artist are required for uploads.");
        }
        
        var storageName = $"{Guid.NewGuid()}_{fileName}";
        var destination = Path.Combine(_basePath, "Pending", storageName);

        await using (var fs = new FileStream(destination, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs);
        }

        await _musicRepository.RequestUploadAsync(userId, title, artist, storageName);
    }
}