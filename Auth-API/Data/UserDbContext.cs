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
        public DbSet<EmployeSalary> EmployeSalary { get; set; }
        public DbSet<StateObligation> StateObligation { get; set; }
        public DbSet<StateObligationsEmploye> StateObligationEmploye { get; set; }
        public DbSet<EmployeSalarySO> EmployeSalarySO { get; set; }
        public DbSet<EmployeSalarySOE> EmployeSalarySOE { get; set; }
        public DbSet<IncomeFromWork> IncomeFromWork { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Postavljanje preciznosti i skale za EmployeeSalary entitet
            modelBuilder.Entity<EmployeSalary>()
                .Property(e => e.MealAllowance)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalary>()
              .Property(e => e.Sickness100)
              .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalary>()
            .Property(e => e.Sickness60)
            .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalary>()
                .Property(e => e.HolidayBonus)
                .HasColumnType("decimal(18,10)");
            modelBuilder.Entity<EmployeSalary>()
                .Property(e => e.Credits)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalary>()
                .Property(e => e.DamageCompensation)
                .HasColumnType("decimal(18,5)");

            // Postavljanje preciznosti i skale za EmployeSalarySOCalculation entitet
            modelBuilder.Entity<EmployeSalarySO>()
                .Property(e => e.DeductionHealth)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySO>()
                .Property(e => e.DeductionPension)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySO>()
                .Property(e => e.GrossSalary)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySO>()
                .Property(e => e.ExpenseOfTheEmployer)
                .HasColumnType("decimal(18,5)");

            // Postavljanje preciznosti i skale za EmployeeSalarySOECalculation entitet
            modelBuilder.Entity<EmployeSalarySOE>()
                .Property(e => e.DeductionHealth)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySOE>()
                .Property(e => e.DeductionPension)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySOE>()
                .Property(e => e.DeductionUnemployment)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySOE>()
               .Property(e => e.DeductionTaxRelief)
               .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySOE>()
                .Property(e => e.GrossSalary)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySOE>()
                .Property(e => e.NetoSalary)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<EmployeSalarySOE>()
                .Property(e => e.ExpenseOfTheEmploye)
                .HasColumnType("decimal(18,5)");

            // Postavljanje preciznosti i skale za Employe entitet
            modelBuilder.Entity<Employe>()
                .Property(e => e.HourlyRate)
                .HasColumnType("decimal(18,5)");

            // Postavljanje preciznosti i skale za StateObligations entitet
            modelBuilder.Entity<StateObligation>()
                .Property(e => e.HealthCare)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<StateObligation>()
                .Property(e => e.PIO)
                .HasColumnType("decimal(18,5)");

            // Postavljanje preciznosti i skale za StateObligationsEmploye entitet
            modelBuilder.Entity<StateObligationsEmploye>()
                .Property(e => e.Tax)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<StateObligationsEmploye>()
                .Property(e => e.TaxRelief)
                .HasColumnType("decimal(18,5)");
            modelBuilder.Entity<StateObligationsEmploye>()
                .Property(e => e.Unemployment)
                .HasColumnType("decimal(18,5)");

            modelBuilder.Entity<IncomeFromWork>(entity =>
            {
                entity.Property(e => e.WorkinHours).HasPrecision(18, 5);
                entity.Property(e => e.Sickness60).HasPrecision(18, 5);
                entity.Property(e => e.Sickness100).HasPrecision(18, 5);
                entity.Property(e => e.AnnualVacation).HasPrecision(18, 5);
                entity.Property(e => e.HolidayHours).HasPrecision(18, 5);
                entity.Property(e => e.OvertimeHours).HasPrecision(18, 5);
                entity.Property(e => e.Credit).HasPrecision(18, 5);
                entity.Property(e => e.Demage).HasPrecision(18, 5);
                entity.Property(e => e.HotMeal).HasPrecision(18, 5);
                entity.Property(e => e.Regres).HasPrecision(18, 5);
                entity.Property(e => e.GrossSalary).HasPrecision(18, 5);
            });

            // Dodavanje inicijalnih podataka za StateObligationsEmploye
            modelBuilder.Entity<StateObligationsEmploye>().HasData(
                new StateObligationsEmploye
                {
                    Id = Guid.NewGuid(),
                    PIO = 0.14m,
                    HealthCare = 0.0515m,
                    Unemployment = 0.0075m,
                    TaxRelief = 25000,
                    Tax = 0.10m
                }
            );

            // Dodavanje inicijalnih podataka za StateObligations
            modelBuilder.Entity<StateObligation>().HasData(
                new StateObligation
                {
                    Id = Guid.NewGuid(),
                    PIO = 0.10m,
                    HealthCare = 0.0515m
                }
            );

            // Veza jedan-na-jedan između EmployeSalary i EmployeSalarySO
            /*modelBuilder.Entity<EmployeSalary>()
                .HasOne(es => es.EmployeSalarySO)
                .WithOne(eso => eso.EmployeSalary)
                .HasForeignKey<EmployeSalarySO>(eso => eso.EmployeSalaryId);
            */
            // Veza jedan-na-jedan između EmployeSalary i EmployeSalarySOE
            /*modelBuilder.Entity<EmployeSalary>()
                .HasOne(es => es.EmployeSalarySOE)
                .WithOne(esoe => esoe.EmployeSalary)
                .HasForeignKey<EmployeSalarySOE>(esoe => esoe.EmployeSalaryId);
            /*
            // Veza jedan-na-više između Employe i EmployeChild
            /*modelBuilder.Entity<Employe>()
                .HasMany(e => e.EmployeChild)
                .WithOne(ec => ec.Employe)
                .HasForeignKey(ec => ec.EmployeId);
            */
            // Veza jedan-na-više između Employe i EmployeSalary
            /*modelBuilder.Entity<Employe>()
                .HasMany(e => e.EmployeSalary)
                .WithOne(es => es.EmployeId)
                .HasForeignKey(es => es.EmployeId);
        */
            }


        private string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
