using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ADMitroSremEmploye.Data
{
    public class UserDbContext : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserDbContext(DbContextOptions<UserDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Employe> Employe { get; set; }
        public DbSet<EmployeChild> EmployeChild { get; set; }
        public DbSet<AnnualLeave> AnnualLeaves { get; set; }

        public override int SaveChanges()
        {
            AuditEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AuditEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList(); // ToList to ensure we're working with a fixed collection

            foreach (var entry in entries)
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    // Logujte nedostatak userId-ja
                    Console.WriteLine("AuditEntities: userId nije pronađen ili je null.");
                    continue; // Preskoči unos ako userId nije dostupan
                }

                // Initialize OldData and NewData to empty string or default object if null
                var oldData = entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                    ? SerializeObject(entry.OriginalValues.ToObject()) ?? ""  // Provide default value if null
                    : "";

                var newData = entry.State == EntityState.Added || entry.State == EntityState.Modified
                    ? SerializeObject(entry.CurrentValues.ToObject()) ?? ""  // Provide default value if null
                    : "";

                // Find the User entity corresponding to userId
                var user = Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    // Logujte da korisnik nije pronađen
                    Console.WriteLine($"AuditEntities: Korisnik sa Id={userId} nije pronađen.");
                    continue; // Preskoči unos ako korisnik nije pronađen
                }

                var auditLog = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    OperationType = entry.State.ToString(),
                    OldData = oldData,
                    NewData = newData,
                    User = user, // Assign the User entity
                    UserId = userId, // Assign the UserId value
                    ChangeDateTime = DateTime.UtcNow
                };

                AuditLogs.Add(auditLog);
            }
        }


        private string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
