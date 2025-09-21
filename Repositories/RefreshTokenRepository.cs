using CryptoWalletApp.Exceptions;
using CryptoWalletApp.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly string _connectionString;

    public RefreshTokenRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public bool IsExistRefreshToken(string refreshToken)
    {
        const string sql = "select CAST(1 AS BIT) from RefreshToken where Token = @refreshToken";
        try
        {
            using var connection = new SqlConnection(_connectionString);
            bool exists = connection.QueryFirstOrDefault<bool>(sql, new { refreshToken = refreshToken });

            if (exists)
                return !IsRevokedRefreshToken(refreshToken);

            return false;
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("Failed to check refresh token existence", ex);
        }
    }

    public bool IsRevokedRefreshToken(string refreshToken)
    {
        try
        {
            const string sql = "select isRevoked from RefreshToken where Token = @refreshToken";
            using var connection = new SqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<bool>(sql, new { refreshToken = refreshToken });
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("failed to check token revoked", ex);
        }
    }

    public UserRoleDto? GetUserRoleDtoFromRefreshToken(string refreshToken)
    {
        const string sql = @"select u.UserID,u.UserName, ur.RoleID
                            from Users u
                            join RefreshToken rt on u.UserID = rt.UserID
                            join UserRole ur on u.UserID = ur.UserID
                            where rt.Token = @refreshToken;";        
        try
        {
            using var connection = new SqlConnection(_connectionString);

            var userRoles = connection.Query<(int UserId, string UserName, string RoleId)>(sql, new { refreshToken });

            var firstUser = userRoles.FirstOrDefault();
            if (firstUser == default)
                throw new Exception("No user with role and refresh token");

            var dto = new UserRoleDto
            {
                UserId = firstUser.UserId,
                UserName = firstUser.UserName,
                RoleIDs = userRoles.Select(r => r.RoleId).ToList()
            };

            return dto;
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("failed to get access token from refresh token", ex);
        }
    }
}
