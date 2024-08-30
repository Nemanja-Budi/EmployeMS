namespace ADMitroSremEmploye.Models.DTOs
{
    public class BankSalaryDto
    {
        public string BankName { get; set; }
        public required string BankAccount { get; set; }
        public decimal TotalNetSalary { get; set; }
    }
}
