using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class TradeTransaction
{
    public int UserId { get; set; }

    public string? TradingAction { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? Tdate { get; set; }

    public string TradingCrypto { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
