using System;
using System.Collections.Generic;

namespace CryptoWalletApp;

public partial class Crypto
{
    public int CryptoId { get; set; }

    public string CryptoName { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public byte FragtionalAmount { get; set; }

    public string? IconPath { get; set; }

    public virtual ICollection<UserCrypto> UserCryptos { get; set; } = new List<UserCrypto>();
}
