using LocalShop.Domain.Models;
using LocalShop.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using LocalShop.Infrastructure.Data;

namespace LocalShop.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
        Task<bool> RegisterAsync(LoginRequest request, string role = "User");
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthService : IAuthService
    {
        private readonly LocalShopDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(LocalShopDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return null;

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return null;

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            var token = _jwtService.GenerateToken(user.Username, user.Email, roles);

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // This should match the JWT expiry
            };
        }

        public async Task<bool> RegisterAsync(LoginRequest request, string role = "User")
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return false;

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return false;

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Email = request.Email ?? $"{request.Username}@example.com"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Get the role
            var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == role);
            if (roleEntity == null)
                return false;

            // Assign role to user
            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = roleEntity.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public string HashPassword(string password)
        {
            //using var sha256 = SHA256.Create();
            //var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            //return Convert.ToBase64String(hashedBytes);
            return password;
        }

        public bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
} 