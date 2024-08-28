using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Services
{
    public class BankSeedService
    {
        private readonly UserDbContext userDbContext;

        public BankSeedService(UserDbContext context)
        {
            userDbContext = context;
        }

        public async Task SeedBankDataAsync()
        {
            if (!await userDbContext.Set<Bank>().AnyAsync())
            {
                var banks = new List<Bank>
            {
                new Bank { Id = Guid.NewGuid(), BankName = "Komercijalna Banka", BankEmail = "komercijalna@bank.com", BankAccount = "908-20501-70" },
                new Bank { Id = Guid.NewGuid(), BankName = "Raiffeisen Banka", BankEmail = "raiffeisen@bank.com", BankAccount = "908-26501-15" },
                new Bank { Id = Guid.NewGuid(), BankName = "Aik Banka", BankEmail = "aik@bank.com", BankAccount = "908-10501-97" },
                new Bank { Id = Guid.NewGuid(), BankName = "Yettel Banka", BankEmail = "yettel@bank.com", BankAccount = "908-11501-07" },
                new Bank { Id = Guid.NewGuid(), BankName = "Euro Bank Direktna", BankEmail = "eurod@bank.com", BankAccount = "908-15001-80" },
                new Bank { Id = Guid.NewGuid(), BankName = "Banka Intesa", BankEmail = "intesa@bank.com", BankAccount = "908-16001-87" },
                new Bank { Id = Guid.NewGuid(), BankName = "OTP Banka", BankEmail = "otp@bank.com", BankAccount = "908-32501-57" },
            };

                userDbContext.Bank.AddRange(banks);
                await userDbContext.SaveChangesAsync();
            }
        }
    }
}