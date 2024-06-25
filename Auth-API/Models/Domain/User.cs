using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADMitroSremEmploye.Models.Domain
{
    public class User : IdentityUser
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        public DateTime DataCreated { get; set; } = DateTime.UtcNow;

    }
}
