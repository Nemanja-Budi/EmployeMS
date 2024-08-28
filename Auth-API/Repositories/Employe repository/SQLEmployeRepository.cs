using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADMitroSremEmploye.Repositories.Employe_repository
{
    public class SQLEmployeRepository : IEmployeRepository
    {
        private readonly UserDbContext userDbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SQLEmployeRepository(UserDbContext userDbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(int totalCount, IEnumerable<Employe>)> GetEmployesAsync(EmployeFilterDto filterDto, CommonFilterDto commonFilterDto)
        {

            var employesQuery = userDbContext.Employe
                .Include(e => e.EmployeChild)
                .Include(e => e.Bank)
                .AsQueryable();


            foreach (var property in typeof(EmployeFilterDto).GetProperties())
            {
                var value = property.GetValue(filterDto);

                if (value != null)
                {

                    if (property.PropertyType == typeof(string))
                    {

                        if (property.Name.Equals("BankName", StringComparison.OrdinalIgnoreCase))
                        {
                            employesQuery = employesQuery.Where(e => EF.Functions.Like(e.Bank.BankName, $"%{value}%"));
                        }
                        else
                        {

                            employesQuery = employesQuery.Where(e => EF.Functions.Like(EF.Property<string>(e, property.Name), $"%{value}%"));
                        }
                    }

                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        employesQuery = employesQuery.Where(e => EF.Property<int?>(e, property.Name) == (int?)value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(commonFilterDto.SortBy))
            {
                var propertyInfo = typeof(Employe).GetProperty(commonFilterDto.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    employesQuery = commonFilterDto.IsAscending
                        ? employesQuery.OrderBy(e => EF.Property<object>(e, propertyInfo.Name))
                        : employesQuery.OrderByDescending(e => EF.Property<object>(e, propertyInfo.Name));
                }
            }

            var employes = await employesQuery
                .Skip((commonFilterDto.PageNumber - 1) * commonFilterDto.PageSize)
                .Take(commonFilterDto.PageSize)
                .ToListAsync();


            var totalCount = await employesQuery.CountAsync();

            return (totalCount, employes);
        }

        public async Task<Employe?> GetEmployeAsync(Guid id)
        {
            return await userDbContext.Employe.Include(al => al.EmployeChild).Include(e => e.Bank).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employe> CreateEmployeAsync(Employe employe)
        {
            var existingBank = await userDbContext.Bank.FirstOrDefaultAsync(b => b.Id == employe.Bank.Id);
            if(existingBank != null)
            {
                employe.Bank = existingBank;
            } 
            else
            {
                userDbContext.Bank.Add(employe.Bank);
            }

            userDbContext.Employe.Add(employe);
            await userDbContext.SaveChangesAsync();

            return employe;
        }

        public async Task<Employe?> UpdateEmployeAsync(Employe employe)
        {
            var existingEmploye = await userDbContext.Employe
                .Include(e => e.EmployeChild)
                .FirstOrDefaultAsync(e => e.Id == employe.Id);

            if (existingEmploye == null)
            {
                return null;
            }

            userDbContext.Entry(existingEmploye).CurrentValues.SetValues(employe);

            foreach (var existingChild in existingEmploye.EmployeChild.ToList())
            {
                if (!employe.EmployeChild.Any(c => c.Name == existingChild.Name))
                {
                    userDbContext.EmployeChild.Remove(existingChild);
                }
            }

            foreach (var child in employe.EmployeChild)
            {
                var existingChild = existingEmploye.EmployeChild.FirstOrDefault(c => c.Name == child.Name);

                if (existingChild != null)
                {
                    userDbContext.Entry(existingChild).CurrentValues.SetValues(child);
                }
                else
                {
                    existingEmploye.EmployeChild.Add(child);
                }
            }

            try
            {
                await userDbContext.SaveChangesAsync();
                return existingEmploye;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeExistsAsync(employe.Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }


        public async Task<bool> DeleteEmployeAsync(Guid id)
        {
            var employe = await userDbContext.Employe
                .Include(e => e.EmployeChild)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employe == null)
            {
                return false;
            }

            // Izbriši decu zaposlenog
            userDbContext.EmployeChild.RemoveRange(employe.EmployeChild);

            // Izbriši zaposlenog
            userDbContext.Employe.Remove(employe);

            await userDbContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> EmployeExistsAsync(Guid id)
        {
            return await userDbContext.Employe.AnyAsync(e => e.Id == id);
        }
    }
}
