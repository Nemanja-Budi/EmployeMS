﻿using ADMitroSremEmploye.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class EmployeDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public required string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Parent's name cannot be longer than 100 characters.")]
        public required string NameOfParent { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "JMBG must be exactly 13 digits.")]
        public required string JMBG { get; set; }

        public decimal HourlyRate { get; set; }
        [Required]
        [RegularExpression(@"^[MF]$", ErrorMessage = "Gender must be either 'M' (Male) or 'F' (Female).")]
        public char Gender { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Identity card number cannot be longer than 20 characters.")]
        public required string IdentityCardNumber { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        public required string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        public required string Address { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Place of birth cannot be longer than 50 characters.")]
        public required string PlaceOfBirth { get; set; }

        [Required]
        public DateOnly DateOfEmployment { get; set; }

        [Required]
        [Range(1000000000, 9999999999, ErrorMessage = "PIO must be a 10-digit number.")]
        public ulong PIO { get; set; }

        [StringLength(100, ErrorMessage = "School name cannot be longer than 100 characters.")]
        public string? School { get; set; }

        [StringLength(100, ErrorMessage = "College name cannot be longer than 100 characters.")]
        public string? College { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Position cannot be longer than 50 characters.")]
        public required string Position { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Employment contract cannot be longer than 50 characters.")]
        public required string EmploymentContract { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Amendment contract cannot be longer than 50 characters.")]
        public required string AmendmentContract { get; set; }

        [Required]
        [Range(1000000000, 9999999999, ErrorMessage = "Current account must be a valid account number.")]
        public ulong EmployeBankAccount { get; set; }

        public EmployeBank? Bank { get; set; }

        public ICollection<EmployeChild>? EmployeChild { get; set; }

    }
}
