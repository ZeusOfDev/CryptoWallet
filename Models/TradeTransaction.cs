using System;
using System.Collections.Generic;

namespace CryptoWalletApp;

public partial class TradeTransaction
{
    public int UserId { get; set; }

    public string? TradingAction { get; set; }

    public string Amount { get; set; } = null!;

    public DateOnly? Tdate { get; set; }

    public string TradingCrypto { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
