﻿using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Otpremnica;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Prijemnica;
using ADMitroSremEmploye.Models.Domain.MP;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz;

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
        public DbSet<Bank> Bank { get; set; }
        
        public DbSet<Dokument> Dokument { get; set; }
        public DbSet<Komintenti> Komintenti { get; set; }
        public DbSet<Proizvod> Proizvod { get; set; }
        
        public DbSet<Racun> Racun { get; set; }
        public DbSet<RacunStavke> RacunStavke { get; set; }

        public DbSet<Otpremnica> Otpremnica { get; set; }
        public DbSet<OtpremnicaStavke> OtpremnicaStavke { get; set; }


        public DbSet<Kalkulacija> Kalkulacija { get; set; }
        public DbSet<KalkulacijaStavke> KalkulacijaStavke { get; set; }
        
        public DbSet<PrijemnicaStavke> PrijemnicaStavke { get; set; }
        public DbSet<Prijemnica> Prijemnica { get; set; }

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
                .ToList(); 

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
                    User = user, 
                    UserId = userId, 
                    ChangeDateTime = DateTime.UtcNow
                };

                AuditLogs.Add(auditLog);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UlazniPodaci>().UseTpcMappingStrategy();
            modelBuilder.Entity<IzlazniPodaci>().UseTpcMappingStrategy();

            modelBuilder.Entity<IzlazniPodaci>(entity =>
            {
                entity.Property(r => r.IzlaznaKolicina).HasColumnType("decimal(18,3)");
                entity.Property(r => r.IzlaznaVrednost).HasColumnType("decimal(18,2)");
                entity.Property(r => r.PDV).HasColumnType("decimal(18,2)");
                entity.Property(r => r.CenaBezPdv).HasColumnType("decimal(18,2)");
                entity.Property(r => r.PdvUDin).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Proizvod>(entity =>
            {
                entity.Property(p => p.CenaProizvoda).HasColumnType("decimal(18,2)");
                entity.Property(p => p.CenaProizvodaBezPdv).HasColumnType("decimal(18,3)");
                entity.Property(p => p.ZaliheProizvoda).HasColumnType("decimal(18,3)");
            });

            modelBuilder.Entity<UlazniPodaci>(entity =>
            {
                entity.Property(p => p.UlaznaKolicina).HasColumnType("decimal(18,3)");
                entity.Property(p => p.UlaznaVrednost).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<KalkulacijaStavke>(entity =>
            {
                entity.Property(k => k.Kolicina).HasColumnType("decimal(18,3)");
                entity.Property(k => k.UlaznaCena).HasColumnType("decimal(18,2)");
                entity.Property(k => k.NabavnaCena).HasColumnType("decimal(18,2)");
                entity.Property(k => k.NabavnaVrednost).HasColumnType("decimal(18,2)");
                entity.Property(k => k.VrednostRobeBezPdv).HasColumnType("decimal(18,2)");
                entity.Property(k => k.PdvUDin).HasColumnType("decimal(18,2)");
                entity.Property(k => k.VrednostRobeSaPdv).HasColumnType("decimal(18,2)");
                entity.Property(k => k.CenaProizvodaBezPdv).HasColumnType("decimal(18,2)");
                entity.Property(k => k.CenaProizvodaSaPdv).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Employe>(entity =>
            {
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,5)");
            });

            modelBuilder.Entity<EmployeSalary>(entity =>
            {
                entity.Property(e => e.MealAllowance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.HolidayBonus).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Credits).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DamageCompensation).HasColumnType("decimal(18,2)");
            });
           
            modelBuilder.Entity<EmployeSalarySO>(entity =>
            {
                entity.Property(e => e.DeductionHealth).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DeductionPension).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GrossSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ExpenseOfTheEmployer).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<EmployeSalarySOE>(entity =>
            {
                entity.Property(e => e.DeductionHealth).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DeductionPension).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DeductionUnemployment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DeductionTaxRelief).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GrossSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NetoSalary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ExpenseOfTheEmploye).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<IncomeFromWork>(entity =>
            {
                entity.Property(e => e.WorkinHours).HasPrecision(18, 2);
                entity.Property(e => e.Sickness60).HasPrecision(18, 2);
                entity.Property(e => e.Sickness100).HasPrecision(18, 2);
                entity.Property(e => e.AnnualVacation).HasPrecision(18, 2);
                entity.Property(e => e.HolidayHours).HasPrecision(18, 2);
                entity.Property(e => e.OvertimeHours).HasPrecision(18, 2);
                entity.Property(e => e.Credit).HasPrecision(18, 2);
                entity.Property(e => e.Demage).HasPrecision(18, 2);
                entity.Property(e => e.HotMeal).HasPrecision(18, 2);
                entity.Property(e => e.Regres).HasPrecision(18, 2);
                entity.Property(e => e.MinuliRad).HasPrecision(18, 2);
                entity.Property(e => e.GrossSalary).HasPrecision(18, 2);
            });

            modelBuilder.Entity<StateObligation>(entity =>
            {
                entity.Property(e => e.HealthCare).HasColumnType("decimal(18,4)");
                entity.Property(e => e.PIO).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<StateObligationsEmploye>(entity =>
            {
                entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxRelief).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Unemployment).HasColumnType("decimal(18,4)");
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

        }

        private string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
