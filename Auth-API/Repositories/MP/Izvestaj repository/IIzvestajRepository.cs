using ADMitroSremEmploye.Models.DTOs.MP;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.MP.Izvestaj_repository
{
    public interface IIzvestajRepository
    {
        Task<Izvestaj> GetIzvestajZaProizvodAsync([FromQuery] string sifra, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate);
    }
}
