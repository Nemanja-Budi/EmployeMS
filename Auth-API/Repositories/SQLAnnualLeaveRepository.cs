using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ADMitroSremEmploye.Repositories
{
    public class SQLAnnualLeaveRepository : IAnnualLeaveRepository
    {
        private readonly UserDbContext _userDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SQLAnnualLeaveRepository(UserDbContext userDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userDbContext = userDbContext;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AnnualLeave>> GetAnnualLeavesAsync()
        {
            return await _userDbContext.AnnualLeaves.Include(al => al.CreatedByUser).Include(al => al.Employe).ToListAsync();
        }

        public async Task<AnnualLeave> GetAnnualLeaveAsync(Guid id)
        {
            return await _userDbContext.AnnualLeaves.Include(al => al.CreatedByUser).Include(al => al.Employe).FirstOrDefaultAsync(al => al.AnnualLeaveId == id);
        }

        public async Task<ActionResult<AnnualLeave>> PostAnnualLeaveAsync(AnnualLeave annualLeave)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existingEmployee = await _userDbContext.Employe.FindAsync(annualLeave.EmployeId);
            if (existingEmployee == null)
            {
                return new BadRequestObjectResult("Zaposlenik sa datim ID-jem ne postoji.");
            }

            annualLeave.CreatedByUserId = userId;
            annualLeave.CreatedDate = DateTime.UtcNow;
            annualLeave.Employe = existingEmployee;

            _userDbContext.AnnualLeaves.Add(annualLeave);
            await _userDbContext.SaveChangesAsync();

            return new OkObjectResult(annualLeave);
        }

        public async Task<bool> PutAnnualLeaveAsync(Guid id, AnnualLeave annualLeave)
        {
            if (id != annualLeave.AnnualLeaveId)
            {
                return false;
            }

            annualLeave.CreatedByUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            annualLeave.CreatedDate = DateTime.UtcNow;

            _userDbContext.Entry(annualLeave).State = EntityState.Modified;

            try
            {
                await _userDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnualLeaveExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public bool AnnualLeaveExistsAsync(Guid id)
        {
            return _userDbContext.AnnualLeaves.Any(e => e.AnnualLeaveId == id);
        }

        public async Task<bool> DeleteAnnualLeaveAsync(Guid id)
        {
            var annualLeave = await _userDbContext.AnnualLeaves.FindAsync(id);

            if (annualLeave == null)
            {
                return false;
            }

            _userDbContext.AnnualLeaves.Remove(annualLeave);
            await _userDbContext.SaveChangesAsync();

            return true;
        }
    }
}
