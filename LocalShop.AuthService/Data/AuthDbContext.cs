using LocalShop.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalShop.AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleId);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            });

            // Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Admin", Description = "Administrator role" },
                new Role { RoleId = 2, Name = "Manager", Description = "Manager role" },
                new Role { RoleId = 3, Name = "User", Description = "Regular user role" }
            );
        }
    }
}
