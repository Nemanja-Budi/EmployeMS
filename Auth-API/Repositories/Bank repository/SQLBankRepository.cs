using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.Bank_repository
{
    public class SQLBankRepository : IBankRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLBankRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<IEnumerable<Bank>> GetBanksAsync()
        {
            var banks = await userDbContext.Bank.ToListAsync();

            return banks;
        }

        public async Task<Bank?> GetBankByIdAsync(Guid id)
        {
            var bank = await userDbContext.Bank.FirstOrDefaultAsync(x => x.Id == id);
            
            if(bank == null)
            {
                return null;
            }

            return bank;
        }

        public async Task<Bank> CreateBankAsync(Bank bank)
        {

            userDbContext.Bank.Add(bank);
            await userDbContext.SaveChangesAsync();

            return bank;
        }

        public async Task<Bank?> UpdateBankAsync(Bank bank)
        {
            var existingBank = await userDbContext.Bank.FirstOrDefaultAsync(x => x.Id == bank.Id);

            if(existingBank == null)
            {
                return null;
            }

            existingBank.BankName = bank.BankName;
            existingBank.BankEmail = bank.BankEmail;
            existingBank.BankAccount = bank.BankAccount;

            userDbContext.Bank.Update(existingBank);

            await userDbContext.SaveChangesAsync();

            return existingBank;
        }



        public async Task<bool> DeleteBankByIdAsync(Guid id)
        {
            var deletedBank = await userDbContext.Bank.FirstOrDefaultAsync(x => x.Id == id);

            if(deletedBank == null)
            {
                return false;
            }

            userDbContext.Bank.Remove(deletedBank);
            await userDbContext.SaveChangesAsync();

            return true;
        }


    }
}
