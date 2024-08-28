using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Member_repository;
using Microsoft.Extensions.Caching.Memory;

namespace ADMitroSremEmploye.Repositories.Employe_repository
{
    public class CachedEmployeRepository : IEmployeRepository
    {
        private readonly SQLEmployeRepository decorated;
        private readonly IMemoryCache memoryCache;

        public CachedEmployeRepository(SQLEmployeRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<(int totalCount, IEnumerable<Employe>)> GetEmployesAsync(EmployeFilterDto filterDto, CommonFilterDto commonFilterDto)
        {
            string cacheKey = $"employes" +
                $"-{filterDto.FirstName}" +
                $"-{filterDto.LastName}" +
                $"-{filterDto.JMBG}" +
                $"-{filterDto.Email}" +
                $"-{filterDto.BankName}" +
                $"-{commonFilterDto.SortBy}" +
                $"-{commonFilterDto.IsAscending}" +
                $"-{commonFilterDto.PageNumber}" +
                $"-{commonFilterDto.PageSize}";
            
            if (!memoryCache.TryGetValue(cacheKey, out (int totalCount, IEnumerable<Employe>) cachedResult))
            {
                cachedResult = await decorated.GetEmployesAsync(filterDto, commonFilterDto);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<Employe?> GetEmployeAsync(Guid id)
        {
            string key = $"employe-{id}";
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetEmployeAsync(id);
            });
        }

        public async Task<Employe> CreateEmployeAsync(Employe employe)
        {
            return await decorated.CreateEmployeAsync(employe);
        }

        public async Task<bool> DeleteEmployeAsync(Guid id)
        {
            return await decorated.DeleteEmployeAsync(id);
        }

        public async Task<bool> EmployeExistsAsync(Guid id)
        {
            return await decorated.EmployeExistsAsync(id);
        }

        public async Task<Employe?> UpdateEmployeAsync(Employe employe)
        {
            string key = $"employe-{employe.Id}";
            memoryCache.Remove(key);
            return await decorated.UpdateEmployeAsync(employe);
        }
    }
}
