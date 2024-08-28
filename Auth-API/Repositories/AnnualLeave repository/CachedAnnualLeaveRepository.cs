using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Audit_repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ADMitroSremEmploye.Repositories.AnnualLeave_repository
{
    public class CachedAnnualLeaveRepository : IAnnualLeaveRepository
    {
        private readonly SQLAnnualLeaveRepository decorated;
        private readonly IMemoryCache memoryCache;

        public CachedAnnualLeaveRepository(SQLAnnualLeaveRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<(int TotalCount, IEnumerable<AnnualLeave> AnnualLeaves)> GetAnnualLeavesAsync(AnnualLeaveFilterDto filterDto, CommonFilterDto commonFilterDto)
        {
            string cacheKey = $"annual-leaves-" +
                $"{filterDto.FirstName}-" +
                $"{filterDto.LastName}-" +
                $"{commonFilterDto.SortBy}" +
                $"-{commonFilterDto.IsAscending}" +
                $"-{commonFilterDto.PageNumber}" +
                $"-{commonFilterDto.PageSize}";
            if (!memoryCache.TryGetValue(cacheKey, out (int totalCount,IEnumerable<AnnualLeave>) cachedResult))
            {
                cachedResult = await decorated.GetAnnualLeavesAsync(filterDto, commonFilterDto);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<AnnualLeave?> GetAnnualLeaveAsync(Guid id)
        {
            string key = $"annual-leave-{id}";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetAnnualLeaveAsync(id);
            });
        }

        public bool AnnualLeaveExistsAsync(Guid id)
        {
            string key = $"annual-leave-exists-{id}";
            return memoryCache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return decorated.AnnualLeaveExistsAsync(id);
            });
        }

        public async Task<bool> DeleteAnnualLeaveAsync(Guid id)
        {
            string key = $"annual-leave-{id}";
            memoryCache.Remove(key);
            return await decorated.DeleteAnnualLeaveAsync(id);
        }

        public async Task<ActionResult<AnnualLeave>> PostAnnualLeaveAsync(AnnualLeave annualLeave)
        {
            return await decorated.PostAnnualLeaveAsync(annualLeave);
        }

        public async Task<bool> PutAnnualLeaveAsync(Guid id, AnnualLeave annualLeave)
        {
            string key = $"annual-leave-{annualLeave.AnnualLeaveId}";
            memoryCache.Remove(key);

            return await decorated.PutAnnualLeaveAsync(id, annualLeave);
        }
    }
}
