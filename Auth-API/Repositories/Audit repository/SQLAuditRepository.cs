using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.Audit_repository
{
    public class SQLAuditRepository : IAuditRepository
    {
        private readonly UserDbContext userDbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SQLAuditRepository(UserDbContext userDbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.userDbContext = userDbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync()
        {
            return await userDbContext.AuditLogs.Include(a => a.User).ToListAsync();
        }

        public async Task<AuditLog> GetAuditLogAsync(Guid id)
        {
            return await userDbContext.AuditLogs.Include(a => a.User).FirstOrDefaultAsync(a => a.AuditLogId == id);
        }

        public async Task<bool> UpdateAuditLogAsync(Guid id, AuditLog auditLog)
        {
            if (id != auditLog.AuditLogId)
            {
                return false;
            }

            if (auditLog.UserId != null)
            {
                auditLog.User = await userDbContext.Users.FindAsync(auditLog.UserId);
            }

            userDbContext.Entry(auditLog).State = EntityState.Modified;

            try
            {
                await userDbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AuditLogExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await userDbContext.Users.FindAsync(userId);
        }

        public async Task<bool> AuditLogExistsAsync(Guid id)
        {
            return await userDbContext.AuditLogs.AnyAsync(e => e.AuditLogId == id);
        }

        public async Task<AuditLog> CreateAuditLogAsync(AuditLog auditLog)
        {
            if (auditLog.UserId != null)
            {
                auditLog.User = await userDbContext.Users.FindAsync(auditLog.UserId);
            }
            userDbContext.AuditLogs.Add(auditLog);
            await userDbContext.SaveChangesAsync();

            return auditLog;
        }

        public async Task<bool> DeleteAuditLogAsync(Guid id)
        {
            var auditLog = await userDbContext.AuditLogs.FindAsync(id);

            if (auditLog == null)
            {
                return false;
            }

            userDbContext.AuditLogs.Remove(auditLog);
            await userDbContext.SaveChangesAsync();

            return true;
        }

    }
}
