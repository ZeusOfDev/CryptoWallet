using System;
using System.Collections.Generic;

namespace CryptoWalletApp;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDesc { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
