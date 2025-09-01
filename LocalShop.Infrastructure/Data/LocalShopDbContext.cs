using LocalShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Infrastructure.Data
{
    public class LocalShopDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public LocalShopDbContext(DbContextOptions<LocalShopDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.Description).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Product>().Property(p => p.Image).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
                  modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", RoleDescription = "System administrator" },
                new Role { RoleId = 2, RoleName = "User", RoleDescription = "Regular user" },
                new Role { RoleId = 3, RoleName = "Manager", RoleDescription = "Team manager" }
            );

            // Seed Users (passwords are hashed for demo purposes)
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "alice", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Email = "alice@example.com" },
                new User { UserId = 2, Username = "bob", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Email = "bob@example.com" },
                new User { UserId = 3, Username = "charlie", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Email = "charlie@example.com" }
            );

            // Seed UserRoles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserRoleId = 1, UserId = 1, RoleId = 1 }, // Alice → Admin
                new UserRole { UserRoleId = 2, UserId = 2, RoleId = 2 }, // Bob → User
                new UserRole { UserRoleId = 3, UserId = 3, RoleId = 3 }  // Charlie → Manager
            );



        }
    }

}

// The current DbContext is already set up for `dotnet ef migrations add InitialCreate --startup-project ../LocalShop`.

