using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Printawyapis.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}