using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

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

        public async Task<IEnumerable<Employe>> GetEmployesAsync(EmployeFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var employesQuery = userDbContext.Employe.Include(e => e.EmployeChild).AsQueryable();

            foreach (var property in typeof(EmployeFilterDto).GetProperties())
            {
                var value = property.GetValue(filterDto);

                if (value != null)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        employesQuery = employesQuery.Where(e => EF.Functions.Like(EF.Property<string>(e, property.Name), $"%{value}%"));
                    }
                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        employesQuery = employesQuery.Where(e => EF.Property<int?>(e, property.Name) == (int?)value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                var propertyInfo = typeof(Employe).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    employesQuery = isAscending
                        ? employesQuery.OrderBy(e => EF.Property<object>(e, propertyInfo.Name))
                        : employesQuery.OrderByDescending(e => EF.Property<object>(e, propertyInfo.Name));
                }
            }

            var employes = await employesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return employes;
        }

        public async Task<int> GetTotalEmployesCountAsync(EmployeFilterDto filterDto)
        {
            var employesQuery = userDbContext.Employe.AsQueryable();

            foreach (var property in typeof(EmployeFilterDto).GetProperties())
            {
                var value = property.GetValue(filterDto);

                if (value != null)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var stringValue = (string)value;
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            employesQuery = employesQuery.Where(e => EF.Functions.Like(EF.Property<string>(e, property.Name), $"%{stringValue}%"));
                        }
                    }
                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        var intValue = (int?)value;
                        if (intValue.HasValue)
                        {
                            employesQuery = employesQuery.Where(e => EF.Property<int?>(e, property.Name) == intValue);
                        }
                    }
                }
            }

            return await employesQuery.CountAsync();
        }


        public async Task<Employe> GetEmployeAsync(Guid id)
        {
            return await userDbContext.Employe.Include(al => al.EmployeChild).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employe> CreateEmployeAsync(Employe employe)
        {
            userDbContext.Employe.Add(employe);
            await userDbContext.SaveChangesAsync();

            return employe;
        }

        public async Task<Employe> UpdateEmployeAsync(Employe employe)
        {
            var existingEmploye = await userDbContext.Employe
                .Include(e => e.EmployeChild)
                .FirstOrDefaultAsync(e => e.Id == employe.Id);

            if (existingEmploye == null)
            {
                return null;
            }

            // Update existing employe with new values
            userDbContext.Entry(existingEmploye).CurrentValues.SetValues(employe);

            // Remove children that are not present in the updated employe
            foreach (var existingChild in existingEmploye.EmployeChild.ToList())
            {
                if (!employe.EmployeChild.Any(c => c.Name == existingChild.Name))
                {
                    userDbContext.EmployeChild.Remove(existingChild);
                }
            }

            // Update or add new children
            foreach (var child in employe.EmployeChild)
            {
                var existingChild = existingEmploye.EmployeChild.FirstOrDefault(c => c.Name == child.Name);

                if (existingChild != null)
                {
                    // Update existing child with new values
                    userDbContext.Entry(existingChild).CurrentValues.SetValues(child);
                }
                else
                {
                    // Add new child to existing employe
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
