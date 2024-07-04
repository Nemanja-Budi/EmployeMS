using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Services
{
    public class SalaryCalculatorService
    {
        private readonly UserDbContext _dbContext;

        public SalaryCalculatorService(UserDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<EmployeSalary?> CalculateSalary(Guid employeId, EmployeSalary input)
        {
            var employe = await _dbContext.Employe.FirstOrDefaultAsync(e => e.Id == employeId);
            if (employe == null)
                return null;

            var incomeFromWork = CalculateGrossSalary(input, employe);

            decimal grossSalary = incomeFromWork.GrossSalary;

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

            // Dodavanje novog zaposlenog u bazu
            await _dbContext.EmployeSalary.AddAsync(employeSalary);
            await _dbContext.SaveChangesAsync();

            // Čuvanje dodatnih informacija o plata i radu
            var salarySO = SaveSalarySO(employeSalary.Id, grossSalary);
            var salarySOE = SaveSalarySOE(employeSalary.Id, grossSalary);
            var ifw = SaveIncomeFromWork(employeSalary.Id, incomeFromWork);

            employeSalary.EmployeSalarySO = salarySO;
            employeSalary.EmployeSalarySOE = salarySOE;
            employeSalary.IncomeFromWork = ifw;

            // Čuvanje dodatnih informacija u bazi
            await _dbContext.SaveChangesAsync();

            return employeSalary;
        }


        private IncomeFromWork CalculateGrossSalary(EmployeSalary input, Employe employe)
        {
            decimal workinHours = input.TotalNumberOfWorkingHours * employe.HourlyRate;
            decimal sickness60 = (input.Sickness60 * 0.65m) * employe.HourlyRate;
            decimal sickness100 = input.Sickness100 * employe.HourlyRate;
            decimal annualVacation = (input.HoursOfAnnualVacation * 1.10687m) * employe.HourlyRate;
            decimal holidayHours = (input.WorkingHoursForTheHoliday * 1.10687m) * employe.HourlyRate;
            decimal overtimeHours = (input.OvertimeHours * 1.01m) * employe.HourlyRate;
            decimal credit = input.Credits;
            decimal demage = input.DamageCompensation;
            decimal hotMeal = input.MealAllowance * input.TotalNumberOfWorkingHours;
            decimal regres = input.HolidayBonus;

            decimal grossSalary = workinHours + sickness60 + sickness100 + annualVacation + holidayHours + overtimeHours + credit + demage + hotMeal + regres;

            var incomeFromWork = new IncomeFromWork
            {
                WorkinHours = workinHours,
                Sickness60 = sickness60,
                Sickness100 = sickness100,
                AnnualVacation = annualVacation,
                HolidayHours = holidayHours,
                OvertimeHours = overtimeHours,
                Credit = credit,
                Demage = demage,
                HotMeal = hotMeal,
                Regres = regres,
                GrossSalary = grossSalary
            };
            
            return incomeFromWork;
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

        private IncomeFromWork SaveIncomeFromWork(Guid employeSalaryId, IncomeFromWork input)
        {
            var incomeFromWork = new IncomeFromWork
            {
                WorkinHours = input.WorkinHours,
                Sickness60 = input.Sickness60,
                Sickness100 = input.Sickness100,
                AnnualVacation = input.AnnualVacation,
                HolidayHours = input.HolidayHours,
                OvertimeHours = input.OvertimeHours,
                Credit = input.Credit,
                Demage = input.Demage,
                HotMeal = input.HotMeal,
                Regres = input.Regres,
                GrossSalary = input.GrossSalary,
                EmployeSalaryId = employeSalaryId
            };

            _dbContext.IncomeFromWork.Add(incomeFromWork);
            _dbContext.SaveChanges();

            return incomeFromWork;
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
                NetoSalary = grossSalary - CalculateNetoSalary(grossSalary,stateObligationsEmploye),
                ExpenseOfTheEmploye = CalculateNetoSalary(grossSalary, stateObligationsEmploye),
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

        public async Task<List<EmployeSalary>?> GetEmployeSalarys(Guid employeId)
        {
            var employe = await _dbContext.Employe.FirstOrDefaultAsync(x => x.Id == employeId);
            if (employe == null) return null;

            var employeSalary = await _dbContext.EmployeSalary
                .Where(x => x.EmployeId == employeId)
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .ToListAsync();
            

            return employeSalary;
        }

        public async Task<EmployeSalary?> GetEmployeSalary(Guid employeSalaryId)
        {
            var employeSalary = await _dbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .FirstOrDefaultAsync(x => x.Id == employeSalaryId);
            if (employeSalary == null) return null;

            return employeSalary;
        }

        public async Task<bool> DeleteEmployeSalarys(Guid employeId)
        {
            var employe = await _dbContext.Employe.FirstOrDefaultAsync(x => x.Id == employeId);
            if (employe == null) return false;

            var employeSalaries = await _dbContext.EmployeSalary
                .Where(x => x.EmployeId == employeId)
                .ToListAsync();

            if (employeSalaries.Count == 0) return false;

            _dbContext.EmployeSalary.RemoveRange(employeSalaries);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEmployeSalary(Guid employeSalaryId)
        {
            var employeSalary = await _dbContext.EmployeSalary
                .FirstOrDefaultAsync(x => x.Id == employeSalaryId);

            if (employeSalary == null) return false;

            _dbContext.EmployeSalary.Remove(employeSalary);
            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}
