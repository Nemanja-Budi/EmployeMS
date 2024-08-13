using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

namespace ADMitroSremEmploye.Repositories.Audit_repository
{
    public class SQLAuditRepository : IAuditRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLAuditRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<(IEnumerable<AuditLog>, int totalCount)> GetAuditLogsAsync(AuditLogFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize)
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

            // Filtriranje po ChangeDateTime
            if (!string.IsNullOrEmpty(filterDto.ChangeDateTime))
            {
                var dateParts = filterDto.ChangeDateTime.Split('-');

                if (dateParts.Length == 1)
                {
                    if (int.TryParse(dateParts[0], out int year))
                    {
                        auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Year == year);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid year format.");
                    }
                }
                else if (dateParts.Length == 2)
                {
                    if (int.TryParse(dateParts[0], out int year) && int.TryParse(dateParts[1], out int month))
                    {
                        auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime.Year == year && a.ChangeDateTime.Month == month);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid year-month format.");
                    }
                }
                else if (dateParts.Length == 3)
                {
                    if (int.TryParse(dateParts[0], out int year) && int.TryParse(dateParts[1], out int month) && int.TryParse(dateParts[2], out int day))
                    {
                        if (month < 1 || month > 12)
                        {
                            throw new ArgumentException("Month must be between 1 and 12.");
                        }

                        if (day == 0)
                        {
                            day = 1;
                        }

                        var daysInMonth = DateTime.DaysInMonth(year, month);
                        if (day < 1 || day > daysInMonth)
                        {
                            throw new ArgumentException("Day is out of range for the given month and year.");
                        }

                        var startDate = new DateTime(year, month, day);
                        var endDate = startDate.AddDays(1);
                        auditLogsQuery = auditLogsQuery.Where(a => a.ChangeDateTime >= startDate && a.ChangeDateTime < endDate);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid date format.");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid ChangeDateTime format.");
                }
            }

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

            var totalCount = await auditLogsQuery.CountAsync();

            var auditLogs = await auditLogsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (auditLogs, totalCount);
        }


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
