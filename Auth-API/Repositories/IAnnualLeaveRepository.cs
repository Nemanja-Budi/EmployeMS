using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories
{
    public interface IAnnualLeaveRepository
    {
        Task<IEnumerable<AnnualLeave>> GetAnnualLeavesAsync();
        Task<AnnualLeave> GetAnnualLeaveAsync(Guid id);
        Task<ActionResult<AnnualLeave>> PostAnnualLeaveAsync(AnnualLeave annualLeave);
        Task<bool> PutAnnualLeaveAsync(Guid id, AnnualLeave annualLeave);
        bool AnnualLeaveExistsAsync(Guid id);
        Task<bool> DeleteAnnualLeaveAsync(Guid id);
    }
}
