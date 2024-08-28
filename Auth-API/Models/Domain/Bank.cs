namespace ADMitroSremEmploye.Models.Domain
{
    public class Bank
    {
        public Guid Id { get; set; }
        public required string BankName { get; set; }
        public required string BankEmail { get; set; }
        public required string BankAccount { get; set; }
    }
}
