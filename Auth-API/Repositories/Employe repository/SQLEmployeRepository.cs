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
                .ThenInclude(es => es.EmployeSalarySO) // Prvi nivo ThenInclude
            .Include(e => e.EmployeSalary)
                .ThenInclude(es => es.EmployeSalarySOE) // Drugi nivo ThenInclude
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
            userDbContext.Entry(employe).State = EntityState.Modified;

            try
            {
                await userDbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await EmployeExistsAsync(employe.Id);
                if (!exists)
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
