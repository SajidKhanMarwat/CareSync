using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs
{
    public record LoginUser_DTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool IsRememberMe { get; set; } = false;
    }
}
