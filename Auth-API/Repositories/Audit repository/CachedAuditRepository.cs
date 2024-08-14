using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Member_repository;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace ADMitroSremEmploye.Repositories.Audit_repository
{
    public class CachedAuditRepository : IAuditRepository
    {
        private readonly SQLAuditRepository decorated;
        private readonly IMemoryCache memoryCache;

        public CachedAuditRepository(SQLAuditRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<bool> AuditLogExistsAsync(Guid id)
        {
            return await decorated.AuditLogExistsAsync(id);
        }

        public async Task<AuditLog?> CreateAuditLogAsync(AuditLog auditLog)
        {
            return await decorated.CreateAuditLogAsync(auditLog);
        }

        public async Task<bool> DeleteAuditLogAsync(Guid id)
        {
            return await decorated.DeleteAuditLogAsync(id);
        }

        public async Task<AuditLog?> GetAuditLogAsync(Guid id)
        {
            string key = $"audit-{id}";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetAuditLogAsync(id);
            });
        }

        public async Task<(IEnumerable<AuditLog>, int totalCount)> GetAuditLogsAsync(AuditLogFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            string cacheKey = $"audits-{filterDto.UserName}-{filterDto.TableName}-{filterDto.OperationType}-{filterDto.ChangeDateTime}-{sortBy}-{isAscending}-{pageNumber}-{pageSize}";
            if (!memoryCache.TryGetValue(cacheKey, out (IEnumerable<AuditLog>, int totalCount) cachedResult))
            {
                cachedResult = await decorated.GetAuditLogsAsync(filterDto, sortBy, isAscending, pageNumber, pageSize);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            string key = $"audit-user-{userId}";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetUserAsync(userId);
            });
            //return await decorated.GetUserAsync(userId);
        }

        public async Task<bool> UpdateAuditLogAsync(Guid id, AuditLog auditLog)
        {
            return await decorated.UpdateAuditLogAsync(id, auditLog);
        }
    }
}
