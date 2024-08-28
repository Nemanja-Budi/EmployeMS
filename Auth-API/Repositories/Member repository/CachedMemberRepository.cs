using ADMitroSremEmploye.Models.Domain;
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
            string key = $"member-{memberAddEditDto.Id}";

            memoryCache.Remove(key);
            return decorated.AddEditMemberAsync(memberAddEditDto);
        }

        public Task<IdentityResult> DeleteMemberAsync(string userId)
        {
            string key = $"member-{userId}";

            memoryCache.Remove(key);
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

        public async Task<(int totalCount, IEnumerable<MemberViewDto>)> GetMembersAsync(MemberFilterDto memberFilterDto, CommonFilterDto commonFilterDto)
        {
            string cacheKey = $"members-" +
                $"{memberFilterDto.UserName}-" +
                $"{memberFilterDto.FirstName}-" +
                $"{memberFilterDto.LastName}-" +
                $"{commonFilterDto.SortBy}-" +
                $"{commonFilterDto.IsAscending}-" +
                $"{commonFilterDto.PageNumber}-" +
                $"{commonFilterDto.PageSize}";

            if (!memoryCache.TryGetValue(cacheKey, out (int totalCount, IEnumerable<MemberViewDto>) cachedResult))
            {
                cachedResult = await decorated.GetMembersAsync(memberFilterDto, commonFilterDto);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<IdentityResult> LockMemberAsync(string userId)
        {
            string key = $"member-{userId}";

            memoryCache.Remove(key);

            return await decorated.LockMemberAsync(userId);
        }

        public async Task<IdentityResult> UnlockMemberAsync(string userId)
        {
            string key = $"member-{userId}";

            memoryCache.Remove(key);

            return  await decorated.UnlockMemberAsync(userId);
        }
    }
}
