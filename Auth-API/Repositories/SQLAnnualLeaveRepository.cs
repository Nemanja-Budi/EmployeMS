using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs.Filters;
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

        /*public async Task<IEnumerable<AnnualLeave>> GetAnnualLeavesAsync()
        {
            return await _userDbContext.AnnualLeaves.Include(al => al.CreatedByUser).Include(al => al.Employe).ToListAsync();
        }
        */

        public async Task<(int TotalCount, IEnumerable<AnnualLeave> AnnualLeaves)> GetAnnualLeavesAsync(AnnualLeaveFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var annualLeaveQuery = _userDbContext.AnnualLeaves
                .Include(al => al.CreatedByUser)
                .Include(al => al.Employe)
                .AsQueryable();

            // Filtriranje
            if (!string.IsNullOrEmpty(filterDto.FirstName) || !string.IsNullOrEmpty(filterDto.LastName))
            {
                annualLeaveQuery = annualLeaveQuery.Where(al =>
                    (string.IsNullOrEmpty(filterDto.FirstName) || EF.Functions.Like(al.Employe.FirstName, $"%{filterDto.FirstName}%")) &&
                    (string.IsNullOrEmpty(filterDto.LastName) || EF.Functions.Like(al.Employe.LastName, $"%{filterDto.LastName}%"))
                );
            }

            // Brojanje ukupnog broja zapisa
            var totalCount = await annualLeaveQuery.CountAsync();

            // Sortiranje
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
                {
                    annualLeaveQuery = isAscending ? annualLeaveQuery.OrderBy(al => al.Employe.FirstName) : annualLeaveQuery.OrderByDescending(al => al.Employe.FirstName);
                }
                else if (sortBy.Equals("LastName", StringComparison.OrdinalIgnoreCase))
                {
                    annualLeaveQuery = isAscending ? annualLeaveQuery.OrderBy(al => al.Employe.LastName) : annualLeaveQuery.OrderByDescending(al => al.Employe.LastName);
                }
                // Dodajte dodatne uslove za sortiranje po drugim kolonama ako je potrebno
            }

            // Paginacija
            var annualLeaves = await annualLeaveQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, annualLeaves);
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
