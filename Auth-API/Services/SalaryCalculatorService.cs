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

        public async Task<EmployeSalary?> CalculateOrUpdateSalary(Guid employeId, EmployeSalary input)
        {
            var employe = await _dbContext.Employe.FirstOrDefaultAsync(e => e.Id == employeId);
            if (employe == null)
                return null;

            var existingSalary = await _dbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .FirstOrDefaultAsync(e => e.Id == input.Id);

            var incomeFromWork = CalculateGrossSalary(input, employe);
            decimal grossSalary = incomeFromWork.GrossSalary;

            if (existingSalary != null)
            {
                existingSalary.TotalNumberOfHours = input.TotalNumberOfHours;
                existingSalary.TotalNumberOfWorkingHours = input.TotalNumberOfWorkingHours;
                existingSalary.HolidayBonus = input.HolidayBonus;
                existingSalary.MealAllowance = input.MealAllowance;
                existingSalary.Sickness100 = input.Sickness100;
                existingSalary.Sickness60 = input.Sickness60;
                existingSalary.HoursOfAnnualVacation = input.HoursOfAnnualVacation;
                existingSalary.WorkingHoursForTheHoliday = input.WorkingHoursForTheHoliday;
                existingSalary.OvertimeHours = input.OvertimeHours;
                existingSalary.Credits = input.Credits;
                existingSalary.DamageCompensation = input.DamageCompensation;

                existingSalary.EmployeSalarySO = SaveSalarySO(existingSalary.Id, grossSalary, true);
                existingSalary.EmployeSalarySOE = SaveSalarySOE(existingSalary.Id, grossSalary, true);
                existingSalary.IncomeFromWork = SaveIncomeFromWork(existingSalary.Id, incomeFromWork, true);
            }
            else
            {
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

                await _dbContext.EmployeSalary.AddAsync(employeSalary);
                await _dbContext.SaveChangesAsync();

                employeSalary.EmployeSalarySO = SaveSalarySO(employeSalary.Id, grossSalary);
                employeSalary.EmployeSalarySOE = SaveSalarySOE(employeSalary.Id, grossSalary);
                employeSalary.IncomeFromWork = SaveIncomeFromWork(employeSalary.Id, incomeFromWork);

                await _dbContext.SaveChangesAsync();

                return employeSalary;
            }

            await _dbContext.SaveChangesAsync();
            return existingSalary;
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

        private IncomeFromWork SaveIncomeFromWork(Guid employeSalaryId, IncomeFromWork input, bool isUpdate = false)
        {
            var incomeFromWork = _dbContext.IncomeFromWork.FirstOrDefault(i => i.EmployeSalaryId == employeSalaryId);

            if (incomeFromWork == null && isUpdate)
            {
                throw new Exception("IncomeFromWork not found for update");
            }
            else if (incomeFromWork == null)
            {
                incomeFromWork = new IncomeFromWork { EmployeSalaryId = employeSalaryId };
                _dbContext.IncomeFromWork.Add(incomeFromWork);
            }

            incomeFromWork.WorkinHours = input.WorkinHours;
            incomeFromWork.Sickness60 = input.Sickness60;
            incomeFromWork.Sickness100 = input.Sickness100;
            incomeFromWork.AnnualVacation = input.AnnualVacation;
            incomeFromWork.HolidayHours = input.HolidayHours;
            incomeFromWork.OvertimeHours = input.OvertimeHours;
            incomeFromWork.Credit = input.Credit;
            incomeFromWork.Demage = input.Demage;
            incomeFromWork.HotMeal = input.HotMeal;
            incomeFromWork.Regres = input.Regres;
            incomeFromWork.GrossSalary = input.GrossSalary;

            _dbContext.SaveChanges();

            return incomeFromWork;
        }

        private EmployeSalarySOE SaveSalarySOE(Guid employeSalaryId, decimal grossSalary, bool isUpdate = false)
        {
            var stateObligationsEmploye = _dbContext.StateObligationEmploye.FirstOrDefault();
            var salarySOE = _dbContext.EmployeSalarySOE.FirstOrDefault(s => s.EmployeSalaryId == employeSalaryId);

            if (salarySOE == null && isUpdate)
            {
                throw new Exception("EmployeSalarySOE not found for update");
            }
            else if (salarySOE == null)
            {
                salarySOE = new EmployeSalarySOE { EmployeSalaryId = employeSalaryId };
                _dbContext.EmployeSalarySOE.Add(salarySOE);
            }

            salarySOE.GrossSalary = grossSalary;
            salarySOE.DeductionPension = grossSalary * stateObligationsEmploye.PIO;
            salarySOE.DeductionHealth = grossSalary * stateObligationsEmploye.HealthCare;
            salarySOE.DeductionUnemployment = grossSalary * stateObligationsEmploye.Unemployment;
            salarySOE.DeductionTaxRelief = (grossSalary - stateObligationsEmploye.TaxRelief) * stateObligationsEmploye.Tax;
            salarySOE.NetoSalary = grossSalary - CalculateNetoSalary(grossSalary, stateObligationsEmploye);
            salarySOE.ExpenseOfTheEmploye = CalculateNetoSalary(grossSalary, stateObligationsEmploye);

            _dbContext.SaveChanges();

            return salarySOE;
        }

        private EmployeSalarySO SaveSalarySO(Guid employeSalaryId, decimal grossSalary, bool isUpdate = false)
        {
            var stateObligations = _dbContext.StateObligation.FirstOrDefault(o => o.PIO == 0.1m);
            var salarySO = _dbContext.EmployeSalarySO.FirstOrDefault(s => s.EmployeSalaryId == employeSalaryId);

            if (salarySO == null && isUpdate)
            {
                throw new Exception("EmployeSalarySO not found for update");
            }
            else if (salarySO == null)
            {
                salarySO = new EmployeSalarySO { EmployeSalaryId = employeSalaryId };
                _dbContext.EmployeSalarySO.Add(salarySO);
            }

            salarySO.GrossSalary = grossSalary;
            salarySO.DeductionPension = grossSalary * stateObligations.PIO;
            salarySO.DeductionHealth = grossSalary * stateObligations.HealthCare;
            salarySO.ExpenseOfTheEmployer = (grossSalary * stateObligations.PIO) + (grossSalary * stateObligations.HealthCare);

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

        public async Task<EmployeSalary?> UpdateEmployeSalary(Guid employeId, EmployeSalary updatedSalary)
        {
            var employe = await _dbContext.Employe.FirstOrDefaultAsync(x => x.Id == employeId);
            if (employe == null) return null;

            var employeSalary = await _dbContext.EmployeSalary.FirstOrDefaultAsync(x => x.Id == updatedSalary.Id && x.EmployeId == employeId);
            if (employeSalary == null) return null;

            employeSalary.TotalNumberOfHours = updatedSalary.TotalNumberOfHours;
            employeSalary.TotalNumberOfWorkingHours = updatedSalary.TotalNumberOfWorkingHours;
            employeSalary.HolidayBonus = updatedSalary.HolidayBonus;
            employeSalary.MealAllowance = updatedSalary.MealAllowance;
            employeSalary.Sickness100 = updatedSalary.Sickness100;
            employeSalary.Sickness60 = updatedSalary.Sickness60;
            employeSalary.HoursOfAnnualVacation = updatedSalary.HoursOfAnnualVacation;
            employeSalary.WorkingHoursForTheHoliday = updatedSalary.WorkingHoursForTheHoliday;
            employeSalary.OvertimeHours = updatedSalary.OvertimeHours;
            employeSalary.Credits = updatedSalary.Credits;
            employeSalary.DamageCompensation = updatedSalary.DamageCompensation;
            employeSalary.EmployeSalarySO = updatedSalary.EmployeSalarySO;
            employeSalary.EmployeSalarySOE = updatedSalary.EmployeSalarySOE;
            employeSalary.IncomeFromWork = updatedSalary.IncomeFromWork;

            _dbContext.EmployeSalary.Update(employeSalary);
            await _dbContext.SaveChangesAsync();

            return employeSalary;
        }

    }
}
