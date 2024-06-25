using System.ComponentModel.DataAnnotations;

namespace ADMitroSremEmploye.Models.Domain
{
    public class AnnualLeave
    {
        public Guid AnnualLeaveId { get; set; }
        
        [Required]
        public required Guid EmployeId { get; set; }
        
        [Required]
        public required Employe Employe { get; set; }
        
        [Required]
        public required DateTime StartDate { get; set; }
        
        [Required]
        public required DateTime EndDate { get; set; }
        
        [Required]
        public required string Reason { get; set; }
        
        [Required]
        public required string Comments { get; set; }

        [Required]
        public required bool Approved { get; set; }

        [Required]
        public required DateTime RequestDate { get; set; }

        [Required]
        public required DateTime ApprovalDate { get; set; }

        [Required]
        public required int TotalDays { get; set; }

        [Required]
        public required int UsedDays { get; set; }

        [Required]
        public required bool IsPaid { get; set; }

        [Required]
        public required bool IsCarryOver { get; set; }

        [Required]
        public required bool IsSickLeave { get; set; }

        public string? CreatedByUserId { get; set; }
        
        public User? CreatedByUser { get; set; }
        
        [Required]
        public required DateTime CreatedDate { get; set; }
    }
}
