using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories
{
    public class SQLAnnualLeaveRepository : IAnnualLeaveRepository
    {
        private readonly UserDbContext _userDbContext;

        public SQLAnnualLeaveRepository(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task<IEnumerable<AnnualLeave>> GetAnnualLeavesAsync()
        {
            return await _userDbContext.AnnualLeaves.Include(al => al.CreatedByUser).Include(al => al.Employe.EmployeChild).ToListAsync();
        }
    }
}
