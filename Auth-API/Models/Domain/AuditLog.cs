namespace ADMitroSremEmploye.Models.Domain
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public string TableName { get; set; }
        public string OperationType { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime ChangeDateTime { get; set; }
    }
}
