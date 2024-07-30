using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

        /*public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync()
        {
            return await userDbContext.AuditLogs.Include(a => a.User).ToListAsync();
        }
        */
        /*public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(AuditLogFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var auditLogsQuery = userDbContext.AuditLogs.Include(a => a.User).AsQueryable();

            foreach (var property in typeof(AuditLogFilterDto).GetProperties())
            {
                var value = property.GetValue(filterDto);

                if (value != null)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var stringValue = value as string;
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            auditLogsQuery = auditLogsQuery.Where(a => EF.Functions.Like(EF.Property<string>(a, property.Name), $"%{stringValue}%"));
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        var dateTimeValue = (DateTime?)value;
                        if (dateTimeValue.HasValue)
                        {
                            auditLogsQuery = auditLogsQuery.Where(a => EF.Property<DateTime?>(a, property.Name) == dateTimeValue);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                var propertyInfo = typeof(AuditLog).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    auditLogsQuery = isAscending
                        ? auditLogsQuery.OrderBy(a => EF.Property<object>(a, propertyInfo.Name))
                        : auditLogsQuery.OrderByDescending(a => EF.Property<object>(a, propertyInfo.Name));
                }
            }

            var auditLogs = await auditLogsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return auditLogs;
        }
        */

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(AuditLogFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var auditLogsQuery = userDbContext.AuditLogs
                .Include(a => a.User)  // Pretpostavljam da je User entitet povezan s AuditLog
                .AsQueryable();

            // Filtriranje po UserName
            if (!string.IsNullOrEmpty(filterDto.UserName))
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.User.UserName.Contains(filterDto.UserName));
            }

            // Filtriranje po TableName
            if (!string.IsNullOrEmpty(filterDto.TableName))
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.TableName.Contains(filterDto.TableName));
            }

            // Filtriranje po OperationType
            if (!string.IsNullOrEmpty(filterDto.OperationType))
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.OperationType.Contains(filterDto.OperationType));
            }

            if (filterDto.ChangeDateTime.HasValue)
            {
                DateTime changeDate = filterDto.ChangeDateTime.Value;
                if (changeDate.Hour == 0 && changeDate.Minute == 0 && changeDate.Second == 0)
                {
                    if (changeDate.Day == 1 && changeDate.Month == 1)
                    {
                        // Filtriranje po godini
                        auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Year == changeDate.Year);
                    }
                    else if (changeDate.Day == 1)
                    {
                        // Filtriranje po mesecu
                        auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Year == changeDate.Year && a.ChangeDateTime.Month == changeDate.Month);
                    }
                    else
                    {
                        // Filtriranje po tačnom datumu
                        auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Date == changeDate.Date);
                    }
                }
                else
                {
                    // Filtriranje po tačnom datumu i vremenu
                    auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime == changeDate);
                }
            }

            // Filtriranje po ChangeDateTime
            /*if (filterDto.ChangeDateTime.HasValue)
            {
                DateTime changeDate;
                bool isValidDate = DateTime.TryParse(filterDto.ChangeDateTime.Value.ToString(), out changeDate);

                if (isValidDate)
                {
                    auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Date == changeDate.Date);
                }
                else
                {
                    // Ako datum nije validan, vrati prazan rezultat
                    return new List<AuditLog>();
                }
            }
            */

            // Sortiranje
            if (!string.IsNullOrEmpty(sortBy))
            {
                var propertyInfo = typeof(AuditLog).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    auditLogsQuery = isAscending
                        ? auditLogsQuery.OrderBy(a => EF.Property<object>(a, propertyInfo.Name))
                        : auditLogsQuery.OrderByDescending(a => EF.Property<object>(a, propertyInfo.Name));
                }
            }

            var auditLogs = await auditLogsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return auditLogs;
        }

        public async Task<int> GetTotalAuditLogsCountAsync(AuditLogFilterDto filterDto)
        {
            var auditLogsQuery = userDbContext.AuditLogs.AsQueryable();

            // Filtriranje po UserName
            if (!string.IsNullOrEmpty(filterDto.UserName))
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.User.UserName.Contains(filterDto.UserName));
            }

            // Filtriranje po TableName
            if (!string.IsNullOrEmpty(filterDto.TableName))
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.TableName.Contains(filterDto.TableName));
            }

            // Filtriranje po OperationType
            if (!string.IsNullOrEmpty(filterDto.OperationType))
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.OperationType.Contains(filterDto.OperationType));
            }

            // Filtriranje po ChangeDateTime
            if (filterDto.ChangeDateTime.HasValue)
            {
                auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Date == filterDto.ChangeDateTime.Value.Date);
            }

            return await auditLogsQuery.CountAsync();
        }

        /*public async Task<int> GetTotalAuditLogsCountAsync(AuditLogFilterDto filterDto)
        {
            var auditLogsQuery = userDbContext.AuditLogs.AsQueryable();

            foreach (var property in typeof(AuditLogFilterDto).GetProperties())
            {
                var value = property.GetValue(filterDto);

                if (value != null)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var stringValue = value as string;
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            auditLogsQuery = auditLogsQuery.Where(a => EF.Functions.Like(EF.Property<string>(a, property.Name), $"%{stringValue}%"));
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        var dateTimeValue = (DateTime?)value;
                        if (dateTimeValue.HasValue)
                        {
                            auditLogsQuery = auditLogsQuery.Where(a => EF.Property<DateTime?>(a, property.Name) == dateTimeValue);
                        }
                    }
                }
            }

            return await auditLogsQuery.CountAsync();
        }
        */
        public async Task<AuditLog?> GetAuditLogAsync(Guid id)
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
                var user = await userDbContext.Users.FindAsync(auditLog.UserId);
                if(user == null)
                {
                    return false;
                }
                auditLog.User = user;
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

        public async Task<User?> GetUserAsync(Guid userId)
        {
            return await userDbContext.Users.FindAsync(userId);
        }

        public async Task<bool> AuditLogExistsAsync(Guid id)
        {
            return await userDbContext.AuditLogs.AnyAsync(e => e.AuditLogId == id);
        }

        public async Task<AuditLog?> CreateAuditLogAsync(AuditLog auditLog)
        {
            if (auditLog.UserId != null)
            {
                var user = await userDbContext.Users.FindAsync(auditLog.UserId);
                if(user == null)
                {
                    return null;
                }
                auditLog.User = user;
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
