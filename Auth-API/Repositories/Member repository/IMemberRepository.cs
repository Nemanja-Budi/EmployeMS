using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ADMitroSremEmploye.Repositories.Member_repository
{
    public interface IMemberRepository
    {
        Task<(int totalCount, IEnumerable<MemberViewDto>)> GetMembersAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 1000);
        Task<MemberAddEditDto?> GetMemberAsync(string id);
        Task<IdentityResult> AddEditMemberAsync(MemberAddEditDto memberAddEditDto);
        Task<IdentityResult> LockMemberAsync(string userId);
        Task<IdentityResult> UnlockMemberAsync(string userId);
        Task<IdentityResult> DeleteMemberAsync(string userId);
    }
}
