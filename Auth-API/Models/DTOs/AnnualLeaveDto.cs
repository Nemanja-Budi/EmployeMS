﻿using ADMitroSremEmploye.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class AnnualLeaveDto
    {
        public Guid AnnualLeaveId { get; set; }

        [Required]
        public required Guid EmployeId { get; set; }

        //[Required]
        //public required Employe Employe { get; set; }

        [Required]
        public required DateOnly StartDate { get; set; }

        [Required]
        public required DateOnly EndDate { get; set; }

        [Required]
        public required string Reason { get; set; }

        [Required]
        public required string Comments { get; set; }

        [Required]
        public required bool Approved { get; set; }

        [Required]
        public required DateOnly RequestDate { get; set; }

        [Required]
        public required DateOnly ApprovalDate { get; set; }

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

        public ViewUserDto? CreatedByUser { get; set; }

        //[Required]
        public DateTime? CreatedDate { get; set; }
    }
}
