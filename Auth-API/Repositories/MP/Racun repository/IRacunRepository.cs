using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun;

namespace ADMitroSremEmploye.Repositories.MP.Racun_repository
{
    public interface IRacunRepository
    {
        Task<Racun> CreateRacunAsync(IzlazCreateRacun izlazCreateRacun);
    }
}
