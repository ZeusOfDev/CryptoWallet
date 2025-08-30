using System;
using System.Collections.Generic;

namespace CryptoWalletApp;

public partial class UserCrypto
{
    public string WalletId { get; set; } = null!;

    public int UserId { get; set; }

    public int CryptoId { get; set; }

    public string Ucbalance { get; set; } = null!;

    public virtual Crypto Crypto { get; set; } = null!;

    public virtual ICollection<Transaction> TransactionReceiverNavigations { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSenderNavigations { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
