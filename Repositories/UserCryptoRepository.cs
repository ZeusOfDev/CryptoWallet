using CryptoWalletApp.Exceptions;
using CryptoWalletApp.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using CryptoWalletApp.Utility;
using System.Runtime.InteropServices;
public class UserCryptoRepository : IUserCryptoRepository
{
    private readonly string _connectionString;

    public UserCryptoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IEnumerable<UserCrypto> GetWalletIDs(int userId)
    {
        const string sql = @"select WalletID, UCBalance from UserCrypto where UserID = @userId";
        try
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<UserCrypto>(sql, new { userId });
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("Failed to get wallet IDs", ex);
        }
    }


    public async Task<IEnumerable<Crypto>> AddWallet(int userId)
    {
        const string sql = @"select CryptoID, CryptoName, ShortName, FragtionalAmount from Crypto
                             where CryptoID not in 
                             (
	                             select uc.CryptoID from UserCrypto uc
	                             where uc.UserID = @UserID
                             )";
        try
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Crypto>(sql, new { userId });
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("Failed to get wallet not belong user", ex);
        }
        
    }

    public bool UserHasWalletType(string symbol, int userId)
    {
        var upperSymbol = symbol.ToUpper();
        const string sql = @"select CAST(1 AS BIT) from UserCrypto uc 
                             join Crypto c on c.CryptoID = uc.CryptoID 
                             where uc.UserID = @userId and c.ShortName = @symbol";
        try
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.QuerySingleOrDefault<bool>(sql, new { userId, symbol = upperSymbol });
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("Failed to check user has wallet type", ex);
        }
    }
    public UserCrypto? GetWalletInfoFromSymbol(string symbol, int userId)
    {
        const string sql = @"SELECT uc.Wallet, uc.UCBalance FROM UserCrypto uc WHERE uc.CryptoID = @symbol AND uc.UserID = @userId";
        try
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<UserCrypto>(sql, new { userId, symbol = symbol });
        }
        catch (SqlException ex)
        {
            throw new DataAccessException("Failed to check user has wallet type", ex);
        }
    }

    public async Task<int> ConvertToUSDT(int userId, string symbol, decimal amount)
    {
        var latestPrice = await PriceHelper.GetPriceAsync(symbol);
        var usdtAmount = amount * latestPrice;
        const string sql = @"BEGIN TRY
                            BEGIN TRANSACTION;
                            UPDATE UserCrypto
                            SET UCBalance = UCBalance - @amount;
                            WHERE CryptoID = @symbol and UserID = @userId;
                        
                            Update UserCrypto       
                            Set UCBalance = UCBalance + @usdtAmount;
                            WHERE CryptoID = @symbol and UserID = @userID;
                            COMMIT TRANSACTION;
                        END TRY
                        BEGIN CATCH
                            ROLLBACK TRANSACTION;
                        END CATCH;";
        try
        {
            using var sqlconnection = new SqlConnection(_connectionString);
            int rowAffected = await sqlconnection.ExecuteAsync(sql, new { userId, symbol });
            return rowAffected;
        }
        catch (SqlException ex)
        {
            throw new Exception("something went wrong when convert to usdt", ex);
        }
    }
}
