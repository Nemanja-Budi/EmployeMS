namespace ADMitroSremEmploye.Models.DTOs
{
    public class BankDto
    {
        public Guid Id { get; set; }
        public required string BankName { get; set; }
        public required string BankEmail { get; set; }
        public required string BankAccount { get; set; }
    }
}
