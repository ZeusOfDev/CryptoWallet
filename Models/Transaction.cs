using System;
using System.Collections.Generic;

namespace CryptoWalletApp;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string Sender { get; set; } = null!;

    public string Receiver { get; set; } = null!;

    public string Amount { get; set; } = null!;

    public DateOnly? Tdate { get; set; }

    public virtual UserCrypto ReceiverNavigation { get; set; } = null!;

    public virtual UserCrypto SenderNavigation { get; set; } = null!;
}
