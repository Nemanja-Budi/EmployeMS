using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories
{
    public interface IAnnualLeaveRepository
    {
        Task<IEnumerable<AnnualLeave>> GetAnnualLeavesAsync();
    }
}
