using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace ADMitroSremEmploye.Repositories.Member_repository
{
    public class CachedMemberRepository : IMemberRepository
    {
        private readonly SQLMemberRepository decorated;
        private readonly IMemoryCache memoryCache;

        public CachedMemberRepository(SQLMemberRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }
        public Task<IdentityResult> AddEditMemberAsync(MemberAddEditDto memberAddEditDto)
        {
            return decorated.AddEditMemberAsync(memberAddEditDto);
        }

        public Task<IdentityResult> DeleteMemberAsync(string userId)
        {
            return decorated.DeleteMemberAsync(userId);
        }

        public async Task<MemberAddEditDto?> GetMemberAsync(string id)
        {
            string key = $"member-{id}";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetMemberAsync(id);
            });
        }

        public async Task<(int totalCount, IEnumerable<MemberViewDto>)> GetMembersAsync(MemberFilterDto memberFilterDto, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            string cacheKey = $"members-{memberFilterDto.UserName}-{memberFilterDto.FirstName}-{memberFilterDto.LastName}-{sortBy}-{isAscending}-{pageNumber}-{pageSize}";

            if (!memoryCache.TryGetValue(cacheKey, out (int totalCount, IEnumerable<MemberViewDto>) cachedResult))
            {
                cachedResult = await decorated.GetMembersAsync(memberFilterDto, sortBy, isAscending, pageNumber, pageSize);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<IdentityResult> LockMemberAsync(string userId)
        {
            return await decorated.LockMemberAsync(userId);
        }

        public async Task<IdentityResult> UnlockMemberAsync(string userId)
        {
            return  await decorated.UnlockMemberAsync(userId);
        }
    }
}
