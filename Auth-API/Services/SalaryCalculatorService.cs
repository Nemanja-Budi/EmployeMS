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
            var employe = _dbContext.Employe.FirstOrDefault(e => e.Id == employeId);
            if (employe == null)
                throw new ArgumentException("Employee not found", nameof(employeId));

            decimal grossSalary = CalculateGrossSalary(input, employe);

            var employeSalary = new EmployeSalary
            {
                EmployeId = employeId,
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

            _dbContext.EmployeSalary.Add(employeSalary);
            _dbContext.SaveChanges();

            var salarySO = SaveSalarySO(employeSalary.Id, grossSalary);
            var salarySOE = SaveSalarySOE(employeSalary.Id, grossSalary);

            employeSalary.EmployeSalarySO = salarySO;
            employeSalary.EmployeSalarySOE = salarySOE;

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
            grossSalary += (input.HoursOfAnnualVacation * 1.10687m) * employe.HourlyRate ;
            grossSalary += (input.WorkingHoursForTheHoliday * 1.10687m) * employe.HourlyRate ;
            grossSalary += (input.OvertimeHours * 1.01m) * employe.HourlyRate;
            grossSalary += input.Credits;
            grossSalary += input.DamageCompensation;
            grossSalary += input.MealAllowance * input.TotalNumberOfWorkingHours;
            grossSalary += input.HolidayBonus;

            return grossSalary;
        }

        private decimal CalculateNetoSalary(decimal grossSalary, StateObligationsEmploye input)
        {
            decimal netoSalary = 0;
            if (grossSalary > 0)
                netoSalary = (grossSalary * input.PIO) 
                                          + (grossSalary * input.HealthCare) 
                                          + (grossSalary * input.Unemployment) 
                                          + ((grossSalary - input.TaxRelief) 
                                          * input.Tax);

            return netoSalary;
        }

        private EmployeSalarySOE SaveSalarySOE(Guid employeSalaryId, decimal grossSalary)
        {
            var stateObligationsEmploye = _dbContext.StateObligationEmploye.FirstOrDefault();

            var salarySOE = new EmployeSalarySOE
            {
                EmployeSalaryId = employeSalaryId,
                GrossSalary = grossSalary,
                DeductionPension = grossSalary * stateObligationsEmploye.PIO,
                DeductionHealth = grossSalary * stateObligationsEmploye.HealthCare,
                DeductionUnemployment = grossSalary * stateObligationsEmploye.Unemployment,
                DeductionTaxRelief = (grossSalary - stateObligationsEmploye.TaxRelief) * stateObligationsEmploye.Tax,
                DeductionTax = CalculateNetoSalary(grossSalary, stateObligationsEmploye),
                NetoSalary = grossSalary - CalculateNetoSalary(grossSalary,stateObligationsEmploye),
                //ExpenseOfTheEmploye = totalDeductions 
            };

            _dbContext.EmployeSalarySOE.Add(salarySOE);
            _dbContext.SaveChanges();

            return salarySOE;
        }

        private EmployeSalarySO SaveSalarySO(Guid employeSalaryId, decimal grossSalary)
        {
            var stateObligations = _dbContext.StateObligation.FirstOrDefault(o => o.PIO == 0.1m);

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

        
    }
}
