using CryptoWalletApp.Models;

public interface IRefreshTokenRepository
{
    bool IsExistRefreshToken(string refreshToken);
    bool IsRevokedRefreshToken(string refreshToken);
    UserRoleDto? GetUserRoleDtoFromRefreshToken(string refreshToken);
}
