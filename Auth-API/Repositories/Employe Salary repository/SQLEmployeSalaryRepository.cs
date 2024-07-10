using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.Employe_Salary_repository
{
    public class SQLEmployeSalaryRepository : IEmployeSalaryRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLEmployeSalaryRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<Employe?> GetEmployeByIdAsync(Guid employeId)
        {
            return await userDbContext.Employe.FirstOrDefaultAsync(x => x.Id == employeId);
        }

        public async Task<List<EmployeSalary>> GetAllEmployeSalarysAsync()
        {
            return await userDbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .ToListAsync();
        }

        public async Task<EmployeSalary?> GetEmployeSalaryById(Guid employeSalaryId)
        {
            return await userDbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .FirstOrDefaultAsync(e => e.Id == employeSalaryId);
        }

        public async Task<EmployeSalary> AddEmployeSalaryAsync(EmployeSalary employeSalary)
        {
            var result = await userDbContext.EmployeSalary.AddAsync(employeSalary);
            await userDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<EmployeSalary>?> GetEmployeSalarysByEmployeIdAsync(Guid employeId)
        {
            return await userDbContext.EmployeSalary
                .Where(x => x.EmployeId == employeId)
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .ToListAsync();
        }

        public async Task<bool> DeleteEmployeSalarysByEmployeIdAsync(Guid employeId)
        {
            var employe = await userDbContext.Employe.FindAsync(employeId);
            if (employe == null) return false;

            var employeSalaries = await userDbContext.EmployeSalary
                .Where(x => x.EmployeId == employeId)
                .ToListAsync();

            if (employeSalaries.Count == 0) return false;

            userDbContext.EmployeSalary.RemoveRange(employeSalaries);
            await userDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteEmployeSalaryByEmployeSalaryIdAsync(Guid employeSalaryId)
        {
            var employeSalary = await userDbContext.EmployeSalary
                .FirstOrDefaultAsync(x => x.Id == employeSalaryId);

            if (employeSalary == null) return false;

            userDbContext.EmployeSalary.Remove(employeSalary);
            await userDbContext.SaveChangesAsync();

            return true;
        }

        public async Task SaveEmployeSalaryAsync()
        {
            await userDbContext.SaveChangesAsync();
        }

    }
}
