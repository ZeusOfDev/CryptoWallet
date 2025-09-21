namespace CryptoWalletApp.Repositories
{
    public class DapperRepository : IDapperRepository
    {
        public IUserCryptoRepository UserCryptoRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }

        public DapperRepository(IConfiguration config)
        {
            UserCryptoRepository = new UserCryptoRepository(config);
            RefreshTokenRepository = new RefreshTokenRepository(config);
        }

    }
}
