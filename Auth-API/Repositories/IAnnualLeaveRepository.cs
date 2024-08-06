using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories
{
    public interface IAnnualLeaveRepository
    {
        //Task<IEnumerable<AnnualLeave>> GetAnnualLeavesAsync();
        Task<(int TotalCount, IEnumerable<AnnualLeave> AnnualLeaves)> GetAnnualLeavesAsync(AnnualLeaveFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<AnnualLeave> GetAnnualLeaveAsync(Guid id);
        Task<ActionResult<AnnualLeave>> PostAnnualLeaveAsync(AnnualLeave annualLeave);
        Task<bool> PutAnnualLeaveAsync(Guid id, AnnualLeave annualLeave);
        bool AnnualLeaveExistsAsync(Guid id);
        Task<bool> DeleteAnnualLeaveAsync(Guid id);
    }
}
