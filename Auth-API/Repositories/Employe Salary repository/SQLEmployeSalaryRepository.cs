using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

        /*public async Task<List<EmployeSalary>> GetAllEmployeSalarysAsync()
        {
            return await userDbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .ToListAsync();
        }
        */

        public async Task<IEnumerable<EmployeSalary>> GetAllEmployeSalarysAsync(EmployeSalaryFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var employeSalaryQuery = userDbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .AsQueryable();

            var employeQuery = userDbContext.Employe.AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.FirstName) || !string.IsNullOrEmpty(filterDto.LastName))
            {
                employeQuery = employeQuery.Where(e =>
                    (string.IsNullOrEmpty(filterDto.FirstName) || EF.Functions.Like(e.FirstName, $"%{filterDto.FirstName}%")) &&
                    (string.IsNullOrEmpty(filterDto.LastName) || EF.Functions.Like(e.LastName, $"%{filterDto.LastName}%"))
                );
            }

            var query = from es in employeSalaryQuery
                        join e in employeQuery on es.EmployeId equals e.Id
                        select new
                        {
                            EmployeSalary = es,
                            Employe = e
                        };

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending ? query.OrderBy(x => x.Employe.FirstName) : query.OrderByDescending(x => x.Employe.FirstName);
                }
                else if (sortBy.Equals("LastName", StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending ? query.OrderBy(x => x.Employe.LastName) : query.OrderByDescending(x => x.Employe.LastName);
                }
            }

            var employeSalaryList = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.EmployeSalary)
                .ToListAsync();

            return employeSalaryList;
        }

        public async Task<int> GetTotalEmployeSalariesCountAsync(EmployeSalaryFilterDto filterDto)
        {
            var employeSalaryQuery = from es in userDbContext.EmployeSalary
                                     join e in userDbContext.Employe on es.EmployeId equals e.Id
                                     select new { es, e };

            if (!string.IsNullOrEmpty(filterDto.FirstName))
            {
                employeSalaryQuery = employeSalaryQuery.Where(es => EF.Functions.Like(es.e.FirstName, $"%{filterDto.FirstName}%"));
            }

            if (!string.IsNullOrEmpty(filterDto.LastName))
            {
                employeSalaryQuery = employeSalaryQuery.Where(es => EF.Functions.Like(es.e.LastName, $"%{filterDto.LastName}%"));
            }

            return await employeSalaryQuery.CountAsync();
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
