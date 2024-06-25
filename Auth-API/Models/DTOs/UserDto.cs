﻿using System.ComponentModel.DataAnnotations;
using System;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class UserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string JWT { get; set; }
    }
}
