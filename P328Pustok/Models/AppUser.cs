﻿using Microsoft.AspNetCore.Identity;

namespace P328Pustok.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
