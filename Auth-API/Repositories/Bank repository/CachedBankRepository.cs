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
        private static readonly object cacheKeysLock = new object();
        private static readonly HashSet<string> bankCacheKeys = new HashSet<string>();

        public CachedBankRepository(SQLBankRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Bank>> GetBanksAsync()
        {
            string key = "banks-list";

            lock (cacheKeysLock)
            {
                bankCacheKeys.Add(key);
            }

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

            lock (cacheKeysLock)
            {
                bankCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetBankByIdAsync(id);
            });
        }

        public Task<Bank> CreateBankAsync(Bank bank)
        {
            RemoveRelatedCache();

            return decorated.CreateBankAsync(bank);
        }

        public Task<Bank?> UpdateBankAsync(Bank bank)
        {
            RemoveRelatedCache();

            return decorated.UpdateBankAsync(bank);
        }

        public Task<bool> DeleteBankByIdAsync(Guid id)
        {
            RemoveRelatedCache();

            return decorated.DeleteBankByIdAsync(id);
        }

        private void RemoveRelatedCache()
        {
            lock (cacheKeysLock)
            {
                foreach (var key in bankCacheKeys)
                {
                    memoryCache.Remove(key);
                }
            }
            bankCacheKeys.Clear();
        }
    }
}
