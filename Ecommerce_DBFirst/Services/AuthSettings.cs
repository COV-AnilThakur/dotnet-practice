namespace Ecommerce_DBFirst.Services
{
    public class AuthSettings
    {
        public List<AuthUser> Users { get; set; } = new();
    }

    public class AuthUser
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string SaltBase64 { get; set; } = string.Empty;
        public string PasswordHashBase64 { get; set; } = string.Empty;
    }
}
