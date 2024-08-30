namespace ADMitroSremEmploye.Models.Domain
{
    public class IncomeFromWork
    {
        public Guid Id { get; set; }
        public decimal WorkinHours { get; set; }
        public decimal Sickness60 {  get; set; }
        public decimal Sickness100 { get; set; }
        public decimal AnnualVacation { get; set; }
        public decimal HolidayHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal Credit { get; set; }
        public decimal Demage { get; set; }
        public decimal HotMeal { get; set; }
        public decimal Regres { get; set; }
        public decimal MinuliRad { get; set; }
        public decimal GrossSalary { get; set; }
        public Guid EmployeSalaryId { get; set; }
    }
}
