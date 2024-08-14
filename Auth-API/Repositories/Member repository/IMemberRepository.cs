using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using Microsoft.AspNetCore.Identity;

namespace ADMitroSremEmploye.Repositories.Member_repository
{
    public interface IMemberRepository
    {
        Task<(int totalCount, IEnumerable<MemberViewDto>)> GetMembersAsync(MemberFilterDto memberFilterDto, CommonFilterDto commonFilterDto);
        Task<MemberAddEditDto?> GetMemberAsync(string id);
        Task<IdentityResult> AddEditMemberAsync(MemberAddEditDto memberAddEditDto);
        Task<IdentityResult> LockMemberAsync(string userId);
        Task<IdentityResult> UnlockMemberAsync(string userId);
        Task<IdentityResult> DeleteMemberAsync(string userId);
    }
}
