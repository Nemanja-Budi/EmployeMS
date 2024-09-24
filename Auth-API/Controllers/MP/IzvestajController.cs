using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP;
using ADMitroSremEmploye.Models.DTOs.MP;
using ADMitroSremEmploye.Repositories.MP.Izvestaj_repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Controllers.MP
{
    [Route("api/mp/[controller]")]
    [ApiController]
    public class IzvestajController : ControllerBase
    {
        private readonly IIzvestajRepository izvestajRepository;

        public IzvestajController(IIzvestajRepository izvestajRepository)
        {
            this.izvestajRepository = izvestajRepository;
        }

        [HttpGet("promet-izvestaj")]
        public async Task<IActionResult> GetIzvestajZaProizvod([FromQuery] string sifra,[FromQuery] DateTime startDate,[FromQuery] DateTime endDate)
        {

            var izvestaj = await izvestajRepository.GetIzvestajZaProizvodAsync(sifra, startDate, endDate);

            return Ok(new { Izvestaj = izvestaj, Vreme = DateTime.UtcNow });
        }


        [HttpGet("izvestaj")]
        public async Task<IActionResult> GetIzvestajZaProizvode([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {

            var izvestaji = await izvestajRepository.GetIzvestajZaProizvodeAsync(startDate, endDate);

            return Ok(new { Izvestaj = izvestaji, Vreme = DateTime.UtcNow });

        }
    }
}
