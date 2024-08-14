using ADMitroSremEmploye.Repositories.Member_repository;
using Microsoft.Extensions.Caching.Memory;

namespace ADMitroSremEmploye.Repositories.Employe_repository
{
    public class CachedEmployeRepository
    {
        private readonly SQLEmployeRepository decorated;
        private readonly IMemoryCache memoryCache;

        public CachedEmployeRepository(SQLEmployeRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }
    }
}
