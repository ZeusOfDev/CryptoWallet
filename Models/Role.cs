using System;
using System.Collections.Generic;

namespace CryptoWalletApp.Models;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string? RoleDesc { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
