using System;
using System.Collections.Generic;

namespace CryptoWalletApp;

public partial class RefreshToken
{
    public string Token { get; set; } = null!;

    public int UserId { get; set; }

    public bool IsRevoked { get; set; }

    public virtual User User { get; set; } = null!;
}
