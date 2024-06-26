using ADMitroSremEmploye.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class AuditLogDto
    {
        public Guid AuditLogId { get; set; }
        [Required]
        public required string TableName { get; set; }
        [Required]
        public required string OperationType { get; set; }
        [Required]
        public required string OldData { get; set; }
        [Required] 
        public required string NewData { get; set; } 
        public User? User { get; set; }
        public string? UserId { get; set; } 
        public DateTime ChangeDateTime { get; set; }
    }
}
