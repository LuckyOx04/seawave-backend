using System.Data;
using Core.Models;
using Dapper;

namespace DataAccess.Repositories;

public class UserRepository(IDbConnectionFactory db)
{
    public async Task RegisterAsync(string username, string email, string passwordHash)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_RegisterUser",
            new { p_username = username, p_email = email, p_passwordHash = passwordHash },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetByLoginAsync(string identifier)
    {
        using var connection = db.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>("sp_GetUserByLogin",
            new { p_identifier = identifier},
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ConfirmEmailAsync(string tokenHash)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_ConfirmEmail",
            new { p_token_hash = tokenHash },
            commandType: CommandType.StoredProcedure) == 1;
    }

    public async Task SetEmailVerificationTokenAsync(int userId, string tokenHash)
    {
        using var connection = db.CreateConnection();
        await connection.ExecuteAsync("sp_SetEmailVerificationToken",
            new { p_user_id = userId, p_token_hash = tokenHash },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ChangePasswordAsync(int userId, string newPasswordHash)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_ChangePassword",
            new { p_user_id = userId, p_new_password_hash = newPasswordHash },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> SetPasswordResetTokenAsync(string email, string tokenHash, int expiryMinutes)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_SetPasswordResetToken",
            new { p_email = email, p_token_hash = tokenHash, p_expiry_minutes = expiryMinutes},
            commandType: CommandType.StoredProcedure) == 1;
    }

    public async Task<bool> ResetPasswordWithTokenAsync(string tokenHash, string newPasswordHash)
    {
        using var connection = db.CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_ResetPasswordWithToken",
            new { p_token_hash = tokenHash, p_new_password_hash = newPasswordHash},
            commandType: CommandType.StoredProcedure) == 1;
    }
}