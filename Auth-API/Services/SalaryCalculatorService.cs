using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using System;
using System.Linq;

namespace ADMitroSremEmploye.Services
{
    public class SalaryCalculatorService
    {
        private readonly UserDbContext _dbContext;

        public SalaryCalculatorService(UserDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public EmployeSalary CalculateSalary(Guid employeId, EmployeSalary input)
        {
            // Fetch employe from database
            var employe = _dbContext.Employe.FirstOrDefault(e => e.Id == employeId);
            if (employe == null)
                throw new ArgumentException("Employee not found", nameof(employeId));

            // Calculate gross salary
            decimal grossSalary = CalculateGrossSalary(input, employe);

            // Calculate deductions based on business logic
            decimal totalDeductions = CalculateDeductions(grossSalary, input);

            // Prepare EmployeSalary entity to return
            var employeSalary = new EmployeSalary
            {
                //Id = Guid.NewGuid(),
                EmployeId = employeId,
                //Employe = employe,
                TotalNumberOfHours = input.TotalNumberOfHours,
                TotalNumberOfWorkingHours = input.TotalNumberOfWorkingHours,
                HolidayBonus = input.HolidayBonus,
                MealAllowance = input.MealAllowance,
                Sickness100 = input.Sickness100,
                Sickness60 = input.Sickness60,
                HoursOfAnnualVacation = input.HoursOfAnnualVacation,
                WorkingHoursForTheHoliday = input.WorkingHoursForTheHoliday,
                OvertimeHours = input.OvertimeHours,
                Credits = input.Credits,
                DamageCompensation = input.DamageCompensation
            };

            // Save EmployeSalary entity to the database first
            _dbContext.EmployeSalary.Add(employeSalary);
            _dbContext.SaveChanges();

            // Save salary calculations to database
            var salarySO = SaveSalarySO(employeSalary.Id, grossSalary);
            var salarySOE = SaveSalarySOE(employeSalary.Id, grossSalary, totalDeductions);

            // Update EmployeSalary with references to SO and SOE
            employeSalary.EmployeSalarySO = salarySO;
            employeSalary.EmployeSalarySOE = salarySOE;

            // Save changes
            _dbContext.SaveChanges();

            return employeSalary;
        }

        private decimal CalculateGrossSalary(EmployeSalary input, Employe employe)
        {
            decimal grossSalary = 0;

            // Calculate gross salary based on provided input and employe hourly rate
            grossSalary += input.TotalNumberOfWorkingHours * employe.HourlyRate;
            grossSalary += (input.Sickness60 * 0.65m) * employe.HourlyRate ;
            grossSalary += input.Sickness100 * employe.HourlyRate;
            grossSalary += (input.HoursOfAnnualVacation * 1.01m) * employe.HourlyRate ;
            grossSalary += (input.WorkingHoursForTheHoliday * 1.01m) * employe.HourlyRate ;
            grossSalary += (input.OvertimeHours * 1.01m) * employe.HourlyRate;
            grossSalary += input.Credits;
            grossSalary += input.DamageCompensation;
            grossSalary += input.MealAllowance * input.TotalNumberOfWorkingHours;
            grossSalary += input.HolidayBonus * employe.HourlyRate;

            return Math.Round(grossSalary, 2);
        }

        private decimal CalculateDeductions(decimal grossSalary, EmployeSalary input)
        {
            decimal totalDeductions = 0;

            // Calculate deductions based on business logic
            // Example calculation (replace with your actual business logic):
            totalDeductions = grossSalary * 0.2m; // Example: 20% deduction

            return totalDeductions;
        }

        private EmployeSalarySO SaveSalarySO(Guid employeSalaryId, decimal grossSalary)
        {
            var stateObligations = _dbContext.StateObligation.FirstOrDefault(o => o.Id.ToString() == "D951C780-B167-4C77-808E-F9BDA8EAED1E");

            Console.WriteLine("***********************");
            Console.WriteLine(stateObligations.PIO);
            Console.WriteLine("***********************");

            var salarySO = new EmployeSalarySO
            {
                EmployeSalaryId = employeSalaryId,
                GrossSalary = grossSalary,
                DeductionPension = grossSalary * stateObligations.PIO, 
                DeductionHealth = grossSalary * stateObligations.HealthCare, 
                ExpenseOfTheEmployer = (grossSalary * stateObligations.PIO) + (grossSalary * stateObligations.HealthCare)
            };

            _dbContext.EmployeSalarySO.Add(salarySO);
            _dbContext.SaveChanges();

            return salarySO;
        }

        private EmployeSalarySOE SaveSalarySOE(Guid employeSalaryId, decimal grossSalary, decimal totalDeductions)
        {
            var stateObligationsEmploye = _dbContext.StateObligationEmploye.FirstOrDefault();

            // Save EmployeSalarySOE entity to the database
            var salarySOE = new EmployeSalarySOE
            {
                //Id = Guid.NewGuid(),
                EmployeSalaryId = employeSalaryId,
                GrossSalary = grossSalary,
                DeductionPension = grossSalary * stateObligationsEmploye.PIO,
                DeductionHealth = grossSalary * stateObligationsEmploye.HealthCare,
                DeductionUnemployment = grossSalary * stateObligationsEmploye.Unemployment,
                DeductionTaxRelief = (grossSalary - stateObligationsEmploye.TaxRelief) * stateObligationsEmploye.Tax,
                DeductionTax = (grossSalary * stateObligationsEmploye.PIO) + (grossSalary * stateObligationsEmploye.HealthCare) + (grossSalary * stateObligationsEmploye.Unemployment) + ((grossSalary - stateObligationsEmploye.TaxRelief) * stateObligationsEmploye.Tax),
                NetoSalary = grossSalary - ((grossSalary * stateObligationsEmploye.PIO) + (grossSalary * stateObligationsEmploye.HealthCare) + (grossSalary * stateObligationsEmploye.Unemployment) + ((grossSalary - stateObligationsEmploye.TaxRelief) * stateObligationsEmploye.Tax)),
                ExpenseOfTheEmploye = totalDeductions
            };

            _dbContext.EmployeSalarySOE.Add(salarySOE);
            _dbContext.SaveChanges();

            return salarySOE;
        }
    }
}
