public interface IDapperRepository
{
    IUserCryptoRepository UserCryptoRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
}
