using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ADMitroSremEmploye.Repositories.Employe_Salary_repository;
using ADMitroSremEmploye.Repositories.State_obligation_repository;
using ADMitroSremEmploye.Repositories.Salary_Service_repository;

namespace ADMitroSremEmploye.Services
{
    public class SalaryCalculatorService
    {
        private readonly IEmployeSalaryRepository employeSalaryRepository;
        private readonly IStateObligationRepository stateObligationRepository;
        private readonly ISalaryServiceRepository salaryServiceRepository;

        public SalaryCalculatorService(
            IEmployeSalaryRepository employeSalaryRepository, 
            IStateObligationRepository stateObligationRepository, 
            ISalaryServiceRepository salaryServiceRepository)
        {
            this.employeSalaryRepository = employeSalaryRepository ?? throw new ArgumentNullException(nameof(employeSalaryRepository));
            this.stateObligationRepository = stateObligationRepository ?? throw new ArgumentNullException(nameof(stateObligationRepository));
            this.salaryServiceRepository = salaryServiceRepository ?? throw new ArgumentNullException(nameof(salaryServiceRepository));
        }

        public async Task<EmployeSalary?> CalculateSalary(EmployeSalary employeSalary)
        {
            var employe = await employeSalaryRepository.GetEmployeByIdAsync(employeSalary.EmployeId);
            
            if (employe == null) return null;

            var existingSalary = await employeSalaryRepository.GetEmployeSalaryById(employeSalary.Id);

            if(existingSalary != null) return null;

            var incomeFromWork = CalculateGrossSalary(employeSalary, employe); 
            decimal grossSalary = incomeFromWork.GrossSalary;

            var EmployeSalary = new EmployeSalary
            {
                EmployeId = employeSalary.EmployeId,
                TotalNumberOfHours = employeSalary.TotalNumberOfHours,
                TotalNumberOfWorkingHours = employeSalary.TotalNumberOfWorkingHours,
                HolidayBonus = employeSalary.HolidayBonus,
                MealAllowance = employeSalary.MealAllowance,
                Sickness100 = employeSalary.Sickness100,
                Sickness60 = employeSalary.Sickness60,
                HoursOfAnnualVacation = employeSalary.HoursOfAnnualVacation,
                WorkingHoursForTheHoliday = employeSalary.WorkingHoursForTheHoliday,
                OvertimeHours = employeSalary.OvertimeHours,
                Credits = employeSalary.Credits,
                DamageCompensation = employeSalary.DamageCompensation
            };

            await employeSalaryRepository.AddEmployeSalaryAsync(EmployeSalary);

            EmployeSalary.EmployeSalarySO = await SaveSalarySO(EmployeSalary.Id, grossSalary);
            EmployeSalary.EmployeSalarySOE = await SaveSalarySOE(EmployeSalary.Id, grossSalary);
            EmployeSalary.IncomeFromWork = await SaveIncomeFromWork(EmployeSalary.Id, incomeFromWork);

            await employeSalaryRepository.SaveEmployeSalaryAsync();

            return EmployeSalary;
        }

        public async Task<EmployeSalary?> UpdateSalary(EmployeSalary employeSalary)
        {
            var employe = await employeSalaryRepository.GetEmployeByIdAsync(employeSalary.EmployeId);

            if (employe == null) return null;

            var existingSalary = await employeSalaryRepository.GetEmployeSalaryById(employeSalary.Id);

            if (existingSalary == null) return null;

            if (employeSalary.EmployeId != existingSalary.EmployeId) return null;


            var incomeFromWork = CalculateGrossSalary(employeSalary, employe);
            decimal grossSalary = incomeFromWork.GrossSalary;

            existingSalary.TotalNumberOfHours = employeSalary.TotalNumberOfHours;
            existingSalary.TotalNumberOfWorkingHours = employeSalary.TotalNumberOfWorkingHours;
            existingSalary.HolidayBonus = employeSalary.HolidayBonus;
            existingSalary.MealAllowance = employeSalary.MealAllowance;
            existingSalary.Sickness100 = employeSalary.Sickness100;
            existingSalary.Sickness60 = employeSalary.Sickness60;
            existingSalary.HoursOfAnnualVacation = employeSalary.HoursOfAnnualVacation;
            existingSalary.WorkingHoursForTheHoliday = employeSalary.WorkingHoursForTheHoliday;
            existingSalary.OvertimeHours = employeSalary.OvertimeHours;
            existingSalary.Credits = employeSalary.Credits;
            existingSalary.DamageCompensation = employeSalary.DamageCompensation;

            existingSalary.EmployeSalarySO = await SaveSalarySO(existingSalary.Id, grossSalary, true);
            existingSalary.EmployeSalarySOE = await SaveSalarySOE(existingSalary.Id, grossSalary, true);
            existingSalary.IncomeFromWork = await SaveIncomeFromWork(existingSalary.Id, incomeFromWork, true);

            await employeSalaryRepository.SaveEmployeSalaryAsync();

            return existingSalary;
        }


        /*public async Task<List<EmployeSalary>> GetAllEmployeSalarys()
        {
            var employeSalary = await employeSalaryRepository.GetAllEmployeSalarysAsync();

            return employeSalary;
        }
        */
        public async Task<List<EmployeSalary>?> GetEmployeSalarys(Guid employeId)
        {
            var employe = await employeSalaryRepository.GetEmployeByIdAsync(employeId);

            if (employe == null) return null;

            var employeSalary = await employeSalaryRepository.GetEmployeSalarysByEmployeIdAsync(employeId);

            return employeSalary;
        }

        public async Task<EmployeSalary?> GetEmployeSalary(Guid employeSalaryId)
        {
            var employeSalary = await employeSalaryRepository.GetEmployeSalaryById(employeSalaryId);
            if (employeSalary == null) return null;

            return employeSalary;
        }

        public async Task<bool> DeleteEmployeSalarys(Guid employeId)
        {
            return await employeSalaryRepository.DeleteEmployeSalarysByEmployeIdAsync(employeId);
        }

        public async Task<bool> DeleteEmployeSalary(Guid employeSalaryId)
        {
            return await employeSalaryRepository.DeleteEmployeSalaryByEmployeSalaryIdAsync(employeSalaryId);
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

        private async Task<IncomeFromWork> SaveIncomeFromWork(Guid employeSalaryId, IncomeFromWork input, bool isUpdate = false)
        {
            var incomeFromWork = await salaryServiceRepository.SaveIncomeFromWorkAsync(employeSalaryId, input, isUpdate);

            return incomeFromWork;
        }

        private async Task<EmployeSalarySOE> SaveSalarySOE(Guid employeSalaryId, decimal grossSalary, bool isUpdate = false)
        {
            var stateObligationsEmploye = await stateObligationRepository.GetStateObligationEmployeAsync() ?? throw new InvalidOperationException("StateObligationsEmploye is not found.");

            var salarySOE = await salaryServiceRepository.SaveSalarySOEAsync(employeSalaryId, grossSalary, stateObligationsEmploye, isUpdate);

            return salarySOE;
        }

        private async Task<EmployeSalarySO> SaveSalarySO(Guid employeSalaryId, decimal grossSalary, bool isUpdate = false)
        {
            var stateObligations = await stateObligationRepository.GetStateObligationAsync() ?? throw new InvalidOperationException("StateObligations is not found.");
            var salarySO = await salaryServiceRepository.SaveSalarySOAsync(employeSalaryId, grossSalary, stateObligations, isUpdate);

            return salarySO;
        }


    }
}
