namespace ADMitroSremEmploye.Models.DTOs.Filters
{
    public class EmployeFilterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? JMBG { get; set; }
        public string? IdentityCardNumber { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public int? PIO { get; set; }
        public string? Position { get; set; }
        public string? EmploymentContract { get; set; }
        public string? AmendmentContract { get; set; }
        public string? BankName { get; set; }
        public int? CurrentAccount { get; set; }
    }
}
