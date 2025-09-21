using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class Crypto
{
    public string CryptoId { get; set; } = null!;

    public string CryptoName { get; set; } = null!;

    public string? IconPath { get; set; }

    public virtual ICollection<UserCrypto> UserCryptos { get; set; } = new List<UserCrypto>();
}
