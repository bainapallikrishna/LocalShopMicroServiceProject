using LocalShop.AuthService.Data;
using LocalShop.AuthService.Models;
using LocalShop.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace LocalShop.AuthService.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> AuthenticateAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request, string? role = "User");
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByUsernameAsync(string username);
    }

    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AuthDbContext context, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponse?> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    return null;
                }

                var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
                var token = _jwtService.GenerateToken(user.Username, roles);

                return new AuthResponse
                {
                    Token = token,
                    Username = user.Username,
                    Roles = roles,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for user: {Username}", request.Username);
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request, string? role = "User")
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(request.Email) && await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return false;
                }

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = HashPassword(request.Password),
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign role
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
                if (roleEntity != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = roleEntity.RoleId
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", request.Username);
                return false;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

                if (user == null) return null;

                return new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email ?? string.Empty,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                return null;
            }
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null) return null;

                return new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email ?? string.Empty,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                return null;
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
