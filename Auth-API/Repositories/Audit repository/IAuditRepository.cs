using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.Audit_repository
{
    public interface IAuditRepository
    {
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync();
        Task<AuditLog?> GetAuditLogAsync(Guid id);
        Task<AuditLog?> CreateAuditLogAsync(AuditLog auditLog);
        Task<bool> UpdateAuditLogAsync(Guid id, AuditLog auditLog);
        Task<User?> GetUserAsync(Guid userId);
        Task<bool> AuditLogExistsAsync(Guid id);
        Task<bool> DeleteAuditLogAsync(Guid id);
    }
}
