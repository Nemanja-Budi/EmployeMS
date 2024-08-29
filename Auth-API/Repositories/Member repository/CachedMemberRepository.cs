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
        private static readonly object cacheKeysLock = new object();
        private static readonly HashSet<string> memberCacheKeys = new HashSet<string>();

        public CachedMemberRepository(SQLMemberRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
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

            lock (cacheKeysLock)
            {
                memberCacheKeys.Add(cacheKey);
            }

            if (!memoryCache.TryGetValue(cacheKey, out (int totalCount, IEnumerable<MemberViewDto>) cachedResult))
            {
                cachedResult = await decorated.GetMembersAsync(memberFilterDto, commonFilterDto);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));

            }

            return cachedResult;
        }

        public async Task<MemberAddEditDto?> GetMemberAsync(string id)
        {
            string key = $"member-{id}";

            lock (cacheKeysLock)
            {
                memberCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetMemberAsync(id);
            });
        }

        public Task<IdentityResult> AddEditMemberAsync(MemberAddEditDto memberAddEditDto)
        {
            RemoveRelatedMemberCache();

            return decorated.AddEditMemberAsync(memberAddEditDto);
        }

        public Task<IdentityResult> DeleteMemberAsync(string userId)
        {
            RemoveRelatedMemberCache();

            return decorated.DeleteMemberAsync(userId);
        }

        public async Task<IdentityResult> LockMemberAsync(string userId)
        {
            RemoveRelatedMemberCache();

            return await decorated.LockMemberAsync(userId);
        }

        public async Task<IdentityResult> UnlockMemberAsync(string userId)
        {
            RemoveRelatedMemberCache();

            return  await decorated.UnlockMemberAsync(userId);
        }

        private void RemoveRelatedMemberCache()
        {
            lock (cacheKeysLock)
            {
                foreach (var key in memberCacheKeys)
                {
                    memoryCache.Remove(key);
                }
            }
            memberCacheKeys.Clear();
        }
    }
}
