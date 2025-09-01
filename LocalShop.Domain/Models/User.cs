using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Domain.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

    }
}
