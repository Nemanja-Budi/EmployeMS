using System;
using System.Collections.Generic;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class MemberViewDto
    {
        public string Id { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool IsLocked { get; set; }
        public DateTime DateCreated { get; set; }
        public required IEnumerable<string> Roles { get; set; }

    }
}
