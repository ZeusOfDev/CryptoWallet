using CryptoWalletApp.Models;

public interface IUserCryptoRepository
{
    IEnumerable<UserCrypto> GetWalletIDs(int userId);
    Task<IEnumerable<Crypto>> AddWallet(int userId);
    bool UserHasWalletType(string symbol, int userId);
    Task<int> ConvertToUSDT(int userId, string symbol, decimal amount);
    UserCrypto? GetWalletInfoFromSymbol(string symbol, int userID);
}
