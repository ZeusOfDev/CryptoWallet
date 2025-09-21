using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class UserCrypto
{
    public string WalletId { get; set; } = null!;

    public int UserId { get; set; }

    public string CryptoId { get; set; } = null!;

    public decimal? Ucbalance { get; set; }

    public virtual Crypto Crypto { get; set; } = null!;

    public virtual ICollection<Transaction> TransactionReceiverNavigations { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSenderNavigations { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
