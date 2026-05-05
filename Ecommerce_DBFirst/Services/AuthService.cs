using Microsoft.Extensions.Options;

namespace Ecommerce_DBFirst.Services
{
    public interface IAuthService
    {
        AuthUser? ValidateCredentials(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AuthSettings _settings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IOptions<AuthSettings> options, ILogger<AuthService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public AuthUser? ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = _settings.Users.FirstOrDefault(u =>
                string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                _logger.LogWarning("Credential validation failed. Username not found: {Username}", username);
                return null;
            }

            var isValid = PasswordHasher.VerifyPassword(password, user.SaltBase64, user.PasswordHashBase64);
            if (!isValid)
            {
                _logger.LogWarning("Credential validation failed. Invalid password for username: {Username}", username);
                return null;
            }

            _logger.LogInformation("Credential validation succeeded for username: {Username}", username);
            return user;
        }
    }
}
