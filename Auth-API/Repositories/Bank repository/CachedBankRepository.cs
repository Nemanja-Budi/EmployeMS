using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Repositories.Audit_repository;
using Microsoft.Extensions.Caching.Memory;

namespace ADMitroSremEmploye.Repositories.Bank_repository
{
    public class CachedBankRepository : IBankRepository
    {
        private readonly SQLBankRepository decorated;
        private readonly IMemoryCache memoryCache;

        public CachedBankRepository(SQLBankRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Bank>> GetBanksAsync()
        {
            string key = "banks-list";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                // Pretpostavljamo da GetBanksAsync nikada ne vraća null
                return await decorated.GetBanksAsync()!;
            });
        }

        public async Task<Bank?> GetBankByIdAsync(Guid id)
        {
            string key = $"bank-{id}";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetBankByIdAsync(id);
            });
        }

        public Task<Bank> CreateBankAsync(Bank bank)
        {
            return decorated.CreateBankAsync(bank);
        }

        public Task<Bank?> UpdateBankAsync(Bank bank)
        {
            string key = $"bank-{bank.Id}";
            memoryCache.Remove(key);
            return decorated.UpdateBankAsync(bank);
        }

        public Task<bool> DeleteBankByIdAsync(Guid id)
        {
            string key = $"bank-{id}";
            memoryCache.Remove(key);
            return decorated.DeleteBankByIdAsync(id);
        }
    }
}
