using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.State_obligation_repository
{
    public class SQLStateObligationRepository : IStateObligationRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLStateObligationRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }
        public async Task<StateObligation?> GetStateObligationAsync()
        {
            var result = await userDbContext.StateObligation.FirstOrDefaultAsync(o => o.PIO == 0.1m);
            
            if(result == null) return null;

            return result;
        }

        public async Task<StateObligationsEmploye?> GetStateObligationEmployeAsync()
        {
            var result = await userDbContext.StateObligationEmploye.FirstOrDefaultAsync();

            if (result == null) return null;

            return result;
        }
    }
}
