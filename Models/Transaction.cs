using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string Sender { get; set; } = null!;

    public string Receiver { get; set; } = null!;

    public decimal? Amount { get; set; }

    public DateOnly? Tdate { get; set; }

    public virtual UserCrypto ReceiverNavigation { get; set; } = null!;

    public virtual UserCrypto SenderNavigation { get; set; } = null!;
}
