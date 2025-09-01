using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Domain.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

    }
}
