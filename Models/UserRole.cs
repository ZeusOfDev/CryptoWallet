using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class UserRole
{
    public int Urid { get; set; }

    public string RoleId { get; set; } = null!;

    public int UserId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
