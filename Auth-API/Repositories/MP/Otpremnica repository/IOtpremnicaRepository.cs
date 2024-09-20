using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Otpremnica;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.MP.Otpremnica_repository
{
    public interface IOtpremnicaRepository
    {
        Task<IEnumerable<Otpremnica>> GetOtpremniceAsync();
        Task<Otpremnica?> GetOtpremnicaByIdAsync(Guid id);
    }
}
