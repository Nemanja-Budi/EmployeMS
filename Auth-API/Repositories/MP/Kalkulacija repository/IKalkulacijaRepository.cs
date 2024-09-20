using ADMitroSremEmploye.Models.Domain.MP.Ulaz;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.MP.Kalkulacija_repository
{
    public interface IKalkulacijaRepository
    {
        Task<IEnumerable<Kalkulacija>> GetKalkulacijeAsync();
        Task<Kalkulacija> CreateKalkulacijaAsync(UlazCreate ulazCreate);
        Task<bool> DeleteKalkulacijaAsync(Guid id);
    }
}
