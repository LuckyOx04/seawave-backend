using System.Data;
using Dapper;

namespace DataAccess.Repositories;

public class SessionRepository(IDbConnectionFactory db)
{
    public async Task CreateSessionAsync(string token, int userId, int expiryDays)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_CreateSession",
            new { p_session_token = token, p_user_id = userId, p_expiry_days = expiryDays },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int?> ValidateSessionAsync(string token)
    {
        using var connection = db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int?>("sp_validateSession",
            new { p_session_token = token },
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteSessionAsync(string token)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_DeleteSession",
            new { p_session_token = token },
            commandType: CommandType.StoredProcedure);
    }
}