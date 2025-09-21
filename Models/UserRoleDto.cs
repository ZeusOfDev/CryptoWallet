namespace CryptoWalletApp.Models
{
    public class UserRoleDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;
        public List<string> RoleIDs { get; set; } = new List<string>();
    }
}
