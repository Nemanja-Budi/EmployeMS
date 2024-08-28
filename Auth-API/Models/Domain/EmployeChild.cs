using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADMitroSremEmploye.Models.Domain
{
    public class EmployeChild
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Child's name cannot be longer than 50 characters.")]
        public required string Name { get; set; }

        [Required]
        public required DateOnly DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "JMBG must be exactly 13 digits.")]
        public required string JMBG { get; set; }

        [Required]
        [RegularExpression(@"^[MF]$", ErrorMessage = "Gender must be either 'M' (Male) or 'F' (Female).")]
        public char Gender { get; set; }
    }
}
