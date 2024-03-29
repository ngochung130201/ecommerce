using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class AddRoleRequest
    {
        public string Email { get; set; }
        public AdminRole Role { get; set; }
    }
}