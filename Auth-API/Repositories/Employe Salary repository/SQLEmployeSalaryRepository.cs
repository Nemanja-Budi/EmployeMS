using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
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

        public async Task<(int totalCount, IEnumerable<EmployeSalary>)> GetAllEmployeSalarysAsync(EmployeSalaryFilterDto filterDto, CommonFilterDto commonFilterDto)
        {
            var employeSalaryQuery = userDbContext.EmployeSalary
                .Include(e => e.EmployeSalarySO)
                .Include(e => e.EmployeSalarySOE)
                .Include(e => e.IncomeFromWork)
                .AsQueryable();

            var employeQuery = userDbContext.Employe.AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.FirstName) || !string.IsNullOrEmpty(filterDto.LastName) || !string.IsNullOrEmpty(filterDto.BankName))
            {
                employeQuery = employeQuery.Where(e =>
                    (string.IsNullOrEmpty(filterDto.FirstName) || EF.Functions.Like(e.FirstName, $"%{filterDto.FirstName}%")) &&
                    (string.IsNullOrEmpty(filterDto.LastName) || EF.Functions.Like(e.LastName, $"%{filterDto.LastName}%")) &&
                    (string.IsNullOrEmpty(filterDto.BankName) || EF.Functions.Like(e.Bank.BankName, $"%{filterDto.BankName}%"))
                );
            }

            if (!string.IsNullOrEmpty(filterDto.CalculationMonth))
            {
                var dateParts = filterDto.CalculationMonth.Split('-');

                if (dateParts.Length == 1)
                {
                    if (int.TryParse(dateParts[0], out int year))
                    {
                        employeSalaryQuery = employeSalaryQuery.Where(e => e.CalculationMonth.Year == year);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid year format.");
                    }
                }
                else if (dateParts.Length == 2)
                {
                    if (int.TryParse(dateParts[0], out int year) && int.TryParse(dateParts[1], out int month))
                    {
                        employeSalaryQuery = employeSalaryQuery.Where(e => e.CalculationMonth.Year == year && e.CalculationMonth.Month == month);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid year-month format.");
                    }
                }
                else if (dateParts.Length == 3)
                {
                    if (int.TryParse(dateParts[0], out int year) && int.TryParse(dateParts[1], out int month) && int.TryParse(dateParts[2], out int day))
                    {
                        if (month < 1 || month > 12)
                        {
                            throw new ArgumentException("Month must be between 1 and 12.");
                        }

                        if (day == 0)
                        {
                            day = 1;
                        }

                        var daysInMonth = DateTime.DaysInMonth(year, month);
                        if (day < 1 || day > daysInMonth)
                        {
                            throw new ArgumentException("Day is out of range for the given month and year.");
                        }

                        var startDate = new DateOnly(year, month, day);
                        var endDate = startDate.AddDays(1);
                        employeSalaryQuery = employeSalaryQuery.Where(e => e.CalculationMonth >= startDate && e.CalculationMonth < endDate);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid date format.");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid ChangeDateTime format.");
                }
            }

            var query = from es in employeSalaryQuery
                        join e in employeQuery on es.EmployeId equals e.Id
                        select new
                        {
                            EmployeSalary = es,
                            Employe = e
                        };

            if (!string.IsNullOrEmpty(commonFilterDto.SortBy))
            {
                if (commonFilterDto.SortBy.Equals("CalculationMonth", StringComparison.OrdinalIgnoreCase))
                {
                    query = commonFilterDto.IsAscending
                        ? query.OrderBy(x => x.EmployeSalary.CalculationMonth)
                        : query.OrderByDescending(x => x.EmployeSalary.CalculationMonth);
                }
                else
                {
                    var propertyInfo = typeof(Employe).GetProperty(commonFilterDto.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        query = commonFilterDto.IsAscending
                            ? query.OrderBy(x => EF.Property<object>(x.Employe, propertyInfo.Name))
                            : query.OrderByDescending(x => EF.Property<object>(x.Employe, propertyInfo.Name));
                    }
                }
            }

            var totalCount = await query.CountAsync();

            var employeSalaryList = await query
                .Skip((commonFilterDto.PageNumber - 1) * commonFilterDto.PageSize)
                .Take(commonFilterDto.PageSize)
                .Select(x => x.EmployeSalary)
                .ToListAsync();

            return (totalCount, employeSalaryList);
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

        public async Task<IEnumerable<BankSalaryDto>?> GetSalariesByBankAsync(int month, int year)
        {
            var salaries = await userDbContext.EmployeSalary
                .Join(
                    userDbContext.Employe,
                    es => es.EmployeId,
                    e => e.Id,
                    (es, e) => new { es, e.Bank.BankName }
                )
                .GroupJoin(
                    userDbContext.EmployeSalarySOE,
                    combined => combined.es.Id,
                    eso => eso.EmployeSalaryId,
                    (combined, esoes) => new { combined.BankName, esoes, combined.es.CalculationMonth }
                )
                .SelectMany(
                    combined => combined.esoes.DefaultIfEmpty(),  // Left join on EmployeSalarySOE
                    (combined, eso) => new { combined.BankName, eso.NetoSalary, combined.CalculationMonth }
                )
                .Where(x => x.CalculationMonth.Month == month && x.CalculationMonth.Year == year)
                .GroupBy(x => x.BankName)
                .Select(group => new BankSalaryDto
                {
                    BankName = group.Key,
                    TotalNetSalary = group.Sum(x => x.NetoSalary)
                })
                .ToListAsync();

            return salaries;
        }

        public async Task<decimal> GetGrandTotalSalaryAsync(int month, int year)
        {
            return await userDbContext.EmployeSalary
                .Join(
                    userDbContext.EmployeSalarySOE,
                    es => es.Id,
                    eso => eso.EmployeSalaryId,
                    (es, eso) => new { eso.NetoSalary, es.CalculationMonth }
                )
                .Where(x => x.CalculationMonth.Month == month && x.CalculationMonth.Year == year)
                .SumAsync(x => x.NetoSalary);
        }

        public async Task SaveEmployeSalaryAsync()
        {
            await userDbContext.SaveChangesAsync();
        }

    }
}
