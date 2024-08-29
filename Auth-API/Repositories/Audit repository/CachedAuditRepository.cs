using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Member_repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace ADMitroSremEmploye.Repositories.Audit_repository
{
    public class CachedAuditRepository : IAuditRepository
    {
        private readonly SQLAuditRepository decorated;
        private readonly IMemoryCache memoryCache;
        private static readonly object cacheKeysLock = new object();
        private static readonly HashSet<string> alCacheKeys = new HashSet<string>();

        public CachedAuditRepository(SQLAuditRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<(IEnumerable<AuditLog>, int totalCount)> GetAuditLogsAsync(AuditLogFilterDto filterDto, CommonFilterDto commonFilterDto)
        {
            string cacheKey = $"audits-" +
                $"{filterDto.UserName}-" +
                $"{filterDto.TableName}-" +
                $"{filterDto.OperationType}-" +
                $"{filterDto.ChangeDateTime}-" +
                $"{commonFilterDto.SortBy}-" +
                $"{commonFilterDto.IsAscending}-" +
                $"{commonFilterDto.PageNumber}-" +
                $"{commonFilterDto.PageSize}";

            lock (cacheKeysLock)
            {
                alCacheKeys.Add(cacheKey);
            }

            if (!memoryCache.TryGetValue(cacheKey, out (IEnumerable<AuditLog>, int totalCount) cachedResult))
            {
                cachedResult = await decorated.GetAuditLogsAsync(filterDto, commonFilterDto);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<AuditLog?> GetAuditLogAsync(Guid id)
        {
            string key = $"audit-{id}";

            lock (cacheKeysLock)
            {
                alCacheKeys.Add(key);
            }


            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetAuditLogAsync(id);
            });
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            string key = $"audit-user-{userId}";

            lock (cacheKeysLock)
            {
                alCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetUserAsync(userId);
            });
        }

        public async Task<bool> AuditLogExistsAsync(Guid id)
        {
            RemoveRelatedCache();
            return await decorated.AuditLogExistsAsync(id);
        }

        public async Task<AuditLog?> CreateAuditLogAsync(AuditLog auditLog)
        {
            RemoveRelatedCache();
            return await decorated.CreateAuditLogAsync(auditLog);
        }

        public async Task<bool> DeleteAuditLogAsync(Guid id)
        {
            RemoveRelatedCache();

            return await decorated.DeleteAuditLogAsync(id);
        }

        public async Task<bool> UpdateAuditLogAsync(Guid id, AuditLog auditLog)
        {
            RemoveRelatedCache();

            return await decorated.UpdateAuditLogAsync(id, auditLog);
        }

        private void RemoveRelatedCache()
        {
            lock (cacheKeysLock)
            {
                foreach (var key in alCacheKeys)
                {
                    memoryCache.Remove(key);
                }
            }
            alCacheKeys.Clear();
        }
    }
}
