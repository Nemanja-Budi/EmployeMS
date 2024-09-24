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
        private readonly UserDbContext userDbContext;
        private readonly IIzvestajRepository izvestajRepository;

        public IzvestajController(UserDbContext userDbContext, IIzvestajRepository izvestajRepository)
        {
            this.userDbContext = userDbContext;
            this.izvestajRepository = izvestajRepository;
        }

        [HttpGet("promet-izvestaj")]
        public async Task<IActionResult> GetIzvestajZaProizvod([FromQuery] string sifra,[FromQuery] DateTime startDate,[FromQuery] DateTime endDate)
        {

            var izvestaj = await izvestajRepository.GetIzvestajZaProizvodAsync(sifra, startDate, endDate);

            return Ok(new { Izvestaj = izvestaj, Vreme = DateTime.UtcNow });
        }



        [HttpGet("izvestaj")]
        public async Task<IActionResult> GetAggregatedData2(
           [FromQuery] DateTime startDate,
           [FromQuery] DateTime endDate)
        {

            var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            // Upit za Prijemnice
            var prijemnice = await userDbContext.Prijemnica
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
                .SelectMany(p => p.PrijemnicaStavke)
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)
                .Select(g => new
                {
                    SifraProizvoda = g.Key,
                    UkupnaUlaznaVrednost = g.Sum(stavka => (decimal?)stavka.UlaznaVrednost) ?? 0,
                    UkupnaUlaznaKolicina = g.Sum(stavka => (decimal?)stavka.UlaznaKolicina) ?? 0
                })
                .ToListAsync();

            // Upit za Kalkulacije
            var kalkulacije = await userDbContext.Kalkulacija
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
                .SelectMany(p => p.KalkulacijaStavke)
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)
                .Select(g => new
                {
                    SifraProizvoda = g.Key,
                    UkupnaUlaznaVrednost = g.Sum(stavka => (decimal?)stavka.UlaznaVrednost) ?? 0,
                    UkupnaUlaznaKolicina = g.Sum(stavka => (decimal?)stavka.UlaznaKolicina) ?? 0
                })
                .ToListAsync();

            // Upit za Otpremnice
            var otpremnice = await userDbContext.Otpremnica
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
                .SelectMany(p => p.OtpremnicaStavke)
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)
                .Select(g => new
                {
                    SifraProizvoda = g.Key,
                    UkupnaIzlaznaVrednost = g.Sum(stavka => (decimal?)stavka.IzlaznaVrednost) ?? 0,
                    UkupnaIzlaznaKolicina = g.Sum(stavka => (decimal?)stavka.IzlaznaKolicina) ?? 0
                })
                .ToListAsync();

            // Upit za Racune
            var racuni = await userDbContext.Racun
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
                .SelectMany(p => p.RacunStavke)
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)
                .Select(g => new
                {
                    SifraProizvoda = g.Key,
                    UkupnaIzlaznaVrednost = g.Sum(stavka => (decimal?)stavka.IzlaznaVrednost) ?? 0,
                    UkupnaIzlaznaKolicina = g.Sum(stavka => (decimal?)stavka.IzlaznaKolicina) ?? 0
                })
                .ToListAsync();

            var rezultati = prijemnice.Concat(kalkulacije)
                .GroupBy(x => x.SifraProizvoda)
                .Select(g => new
                {
                    SifraProizvoda = g.Key,
                    UkupnaUlaznaVrednost = g.Sum(x => x.UkupnaUlaznaVrednost),
                    UkupnaUlaznaKolicina = g.Sum(x => x.UkupnaUlaznaKolicina),
                    UkupnaIzlaznaVrednost = otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaVrednost),
                    UkupnaIzlaznaKolicina = otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaKolicina),
                    RazlikaVrednosti = g.Sum(x => x.UkupnaUlaznaVrednost) - otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaVrednost),
                    RazlikaKolicina = g.Sum(x => x.UkupnaUlaznaKolicina) - otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaKolicina),
                    ProsecnaVrednost = g.Sum(x => x.UkupnaUlaznaKolicina) - otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaKolicina) == 0 ?
                    (decimal?)null :
                    (g.Sum(x => x.UkupnaUlaznaVrednost) - otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaVrednost)) /
                    (g.Sum(x => x.UkupnaUlaznaKolicina) - otpremnice.Concat(racuni)
                        .Where(o => o.SifraProizvoda == g.Key)
                        .Sum(o => o.UkupnaIzlaznaKolicina))
                })
                .ToList();

            // Kreiranje liste proizvoda sa nazivom i jedinicom mere
            var listaProizvoda = await userDbContext.Proizvod
                .Select(p => new ProizvodIzvestaj
                {
                    SifraProizvoda = p.SifraProizvoda,
                    NazivProizvoda = p.NazivProizvoda,
                    JM = p.JM
                })
                .ToListAsync();

            // Spoj rezultata sa proizvodima na osnovu šifre proizvoda
            var spojeniRezultati = rezultati.Join(listaProizvoda,
                r => r.SifraProizvoda,
                p => p.SifraProizvoda,
                (r, p) => new Izvestaj
                {
                    Proizvod = p,
                    UkupnaUlaznaVrednost = r.UkupnaUlaznaVrednost,
                    UkupnaUlaznaKolicina = r.UkupnaUlaznaKolicina,
                    UkupnaIzlaznaVrednost = r.UkupnaIzlaznaVrednost,
                    UkupnaIzlaznaKolicina = r.UkupnaIzlaznaKolicina,
                    RazlikaVrednosti = r.RazlikaVrednosti,
                    RazlikaKolicina = r.RazlikaKolicina,
                    ProsecnaVrednost = r.ProsecnaVrednost
                }).ToList();

            return Ok(new { Izvestaj = spojeniRezultati, Vreme = DateTime.UtcNow });

        }
    }
}
