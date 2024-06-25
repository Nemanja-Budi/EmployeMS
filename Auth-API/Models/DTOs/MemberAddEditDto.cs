using System.ComponentModel.DataAnnotations;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class MemberAddEditDto
    {
        public string Id { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }
        public string? Password { get; set; }

        [Required]
        public required string Roles { get; set; }


    }
}
