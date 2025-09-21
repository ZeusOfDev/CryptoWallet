using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? UserDesc { get; set; }

    public decimal? UserBalance { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<TradeTransaction> TradeTransactions { get; set; } = new List<TradeTransaction>();

    public virtual ICollection<UserCrypto> UserCryptos { get; set; } = new List<UserCrypto>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
