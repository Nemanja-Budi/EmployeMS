using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.Salary_Service_repository
{
    public class SalaryServiceRepository : ISalaryServiceRepository
    {
        private readonly UserDbContext userDbContext;

        public SalaryServiceRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        //IncomeFromWork
        public async Task<IncomeFromWork?> GetIncomFromWorkByEmployeSalaryIdAsync(Guid employeSalaryId)
        {
            var result = await userDbContext.IncomeFromWork.FirstOrDefaultAsync(i => i.EmployeSalaryId == employeSalaryId);
            
            if (result == null) return null;
            
            return result;
        }

        public async Task<IncomeFromWork> AddIncomeFromWorkAsync(IncomeFromWork incomeFromWork)
        {
            await userDbContext.IncomeFromWork.AddAsync(incomeFromWork);
            //await userDbContext.SaveChangesAsync();
            return incomeFromWork;
        }

        public async Task<IncomeFromWork> SaveIncomeFromWorkAsync(Guid employeSalaryId, IncomeFromWork input, bool isUpdate = false)
        {
            var incomeFromWork = await GetIncomFromWorkByEmployeSalaryIdAsync(employeSalaryId);

            if (incomeFromWork == null && isUpdate)
            {
                throw new Exception("IncomeFromWork not found for update");
            }
            else if (incomeFromWork == null)
            {
                incomeFromWork = new IncomeFromWork { EmployeSalaryId = employeSalaryId };
                await AddIncomeFromWorkAsync(incomeFromWork);
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
            incomeFromWork.MinuliRad = input.MinuliRad;
            incomeFromWork.GrossSalary = input.GrossSalary;


            await SaveChangesAsync();

            return incomeFromWork;
        }


        //EmployeSalarySOE
        public async Task<EmployeSalarySOE?> GetEmployeSalarySOEByEmployeSalaryIdAsync(Guid employeSalaryId)
        {
            var result = await userDbContext.EmployeSalarySOE.FirstOrDefaultAsync(i => i.EmployeSalaryId == employeSalaryId);

            if (result == null) return null;

            return result;
        }

        public async Task<EmployeSalarySOE> AddEmployeSalarySOEAsync(EmployeSalarySOE employeSalarySOE)
        {
            await userDbContext.EmployeSalarySOE.AddAsync(employeSalarySOE);
            //await userDbContext.SaveChangesAsync();
            return employeSalarySOE;
        }

        public async Task<EmployeSalarySOE> SaveSalarySOEAsync(Guid employeSalaryId, decimal grossSalary, StateObligationsEmploye stateObligationsEmploye, bool isUpdate = false)
        {
            var salarySOE = await GetEmployeSalarySOEByEmployeSalaryIdAsync(employeSalaryId);

            if (salarySOE == null && isUpdate)
            {
                throw new Exception("EmployeSalarySOE not found for update");
            }
            else if (salarySOE == null)
            {
                salarySOE = new EmployeSalarySOE { EmployeSalaryId = employeSalaryId };
                await AddEmployeSalarySOEAsync(salarySOE);
            }

            salarySOE.GrossSalary = grossSalary;
            salarySOE.DeductionPension = grossSalary * stateObligationsEmploye.PIO;
            salarySOE.DeductionHealth = grossSalary * stateObligationsEmploye.HealthCare;
            salarySOE.DeductionUnemployment = grossSalary * stateObligationsEmploye.Unemployment;
            salarySOE.DeductionTaxRelief = (grossSalary - stateObligationsEmploye.TaxRelief) * stateObligationsEmploye.Tax;
            salarySOE.NetoSalary = grossSalary - CalculateNetoSalary(grossSalary, stateObligationsEmploye);
            salarySOE.ExpenseOfTheEmploye = CalculateNetoSalary(grossSalary, stateObligationsEmploye);

            await SaveChangesAsync();

            return salarySOE;
        }

        //EmployeSalarySO

        public async Task<EmployeSalarySO?> GetEmployeSalarySOByEmployeSalaryIdAsync(Guid employeSalaryId)
        {
            var result = await userDbContext.EmployeSalarySO.FirstOrDefaultAsync(i => i.EmployeSalaryId == employeSalaryId);

            if (result == null) return null;

            return result;
        }

        public async Task<EmployeSalarySO> AddEmployeSalarySOAsync(EmployeSalarySO employeSalarySO)
        {
            await userDbContext.EmployeSalarySO.AddAsync(employeSalarySO);
            //await userDbContext.SaveChangesAsync();
            return employeSalarySO;
        }

        public async Task<EmployeSalarySO> SaveSalarySOAsync(Guid employeSalaryId, decimal grossSalary, StateObligation stateObligation, bool isUpdate = false)
        {
            var salarySO = await userDbContext.EmployeSalarySO.FirstOrDefaultAsync(s => s.EmployeSalaryId == employeSalaryId);

            if (salarySO == null && isUpdate)
            {
                throw new Exception("EmployeSalarySO not found for update");
            }
            else if (salarySO == null)
            {
                salarySO = new EmployeSalarySO { EmployeSalaryId = employeSalaryId };
                await userDbContext.EmployeSalarySO.AddAsync(salarySO);
            }

            salarySO.GrossSalary = grossSalary;
            salarySO.DeductionPension = grossSalary * stateObligation.PIO;
            salarySO.DeductionHealth = grossSalary * stateObligation.HealthCare;
            salarySO.ExpenseOfTheEmployer = (grossSalary * stateObligation.PIO) + (grossSalary * stateObligation.HealthCare);

            await SaveChangesAsync();

            return salarySO;
        }

        // SAVE CHANGES
        public async Task SaveChangesAsync()
        {
            await userDbContext.SaveChangesAsync();
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


    }
}
