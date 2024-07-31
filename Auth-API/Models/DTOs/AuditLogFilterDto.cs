namespace ADMitroSremEmploye.Models.DTOs
{
    public class AuditLogFilterDto
    {
        public string? UserName { get; set; }
        public string? TableName { get; set; }
        public string? OperationType {  get; set; }
        public string? ChangeDateTime { get; set; }

    }
}
