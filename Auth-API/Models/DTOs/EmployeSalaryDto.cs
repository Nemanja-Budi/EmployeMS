using ADMitroSremEmploye.Models.Domain;

namespace ADMitroSremEmploye.Models.DTOs
{
    public class EmployeSalaryDto
    {
        public Guid Id { get; set; }
        public int TotalNumberOfHours { get; set; }
        public int TotalNumberOfWorkingHours { get; set; }
        public decimal HolidayBonus { get; set; } = 1426.53m;
        public decimal MealAllowance { get; set; } = 42.8m;
        public decimal Sickness100 { get; set; } = 0;
        public decimal Sickness60 { get; set; } = 0;
        public int HoursOfAnnualVacation { get; set; } = 0;
        public int WorkingHoursForTheHoliday { get; set; } = 0;
        public int OvertimeHours { get; set; } = 0;
        public decimal Credits { get; set; } = 0m;
        public decimal DamageCompensation { get; set; } = 0m;
        public Guid EmployeId { get; set; }
        public EmployeSalarySO? EmployeSalarySO { get; set; }
        public EmployeSalarySOE? EmployeSalarySOE { get; set; }
        public IncomeFromWork? IncomeFromWork { get; set; }
    }
}
