using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Otpremnica;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.MP.Otpremnica_repository
{
    public class SQLOtpremnicaRepository : IOtpremnicaRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLOtpremnicaRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<IEnumerable<Otpremnica>> GetOtpremniceAsync()
        {
            var otpremnice = await userDbContext.Otpremnica
                .Include(k => k.OtpremnicaStavke)
                    .ThenInclude(ks => ks.Proizvod)
                .Include(k => k.Dokument)
                .ToListAsync();

            return otpremnice;
        }

        public async Task<Otpremnica?> GetOtpremnicaByIdAsync(Guid id)
        {
            var otpremnica = await userDbContext.Otpremnica
                .Include(k => k.OtpremnicaStavke)
                    .ThenInclude(ks => ks.Proizvod)
                .Include(k => k.Dokument)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (otpremnica == null) return null;

            return otpremnica;
        }

    }
}
