    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Application.DTOs.User
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; }
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
       
    }
  
}
