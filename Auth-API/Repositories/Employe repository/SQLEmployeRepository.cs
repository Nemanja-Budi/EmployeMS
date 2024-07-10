using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Employe>> GetEmployesAsync()
        {
            return await userDbContext.Employe
            .Include(e => e.EmployeChild)
            .Include(e => e.EmployeSalary)
                .ThenInclude(es => es.EmployeSalarySO)
            .Include(e => e.EmployeSalary)
                .ThenInclude(es => es.EmployeSalarySOE)
            .ToListAsync();

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

        public async Task<bool> UpdateEmployeAsync(Employe employe)
        {
            var existingEmploye = await userDbContext.Employe
                .Include(e => e.EmployeChild)
                .FirstOrDefaultAsync(e => e.Id == employe.Id);

            if (existingEmploye == null)
            {
                return false;
            }

            userDbContext.Entry(existingEmploye).CurrentValues.SetValues(employe);

            // Update EmployeChild entities
            foreach (var existingChild in existingEmploye.EmployeChild.ToList())
            {
                if (!employe.EmployeChild.Any(c => c.Id == existingChild.Id))
                {
                    userDbContext.EmployeChild.Remove(existingChild);
                }
            }

            foreach (var child in employe.EmployeChild)
            {
                var existingChild = existingEmploye.EmployeChild
                    .FirstOrDefault(c => c.Id == child.Id);

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
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeExistsAsync(employe.Id))
                {
                    return false;
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

            var employeChildren = await userDbContext.EmployeChild
                .Where(c => c.Id == id)
                .ToListAsync();

            userDbContext.EmployeChild.RemoveRange(employeChildren);

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
