using ADMitroSremEmploye.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Controllers.MP
{
    [Route("api/[controller]")]
    [ApiController]
    public class IzvestajController : ControllerBase
    {
        private readonly UserDbContext userDbContext;

        public IzvestajController(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        [HttpGet("promet-izvestaj")]
        public async Task<IActionResult> GetAggregatedData(
            [FromQuery] string sifra,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            // Upit za Prijemnice
            var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            var prijemnice = await userDbContext.Prijemnica
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                            && p.PrijemnicaStavke.Any(x => x.Proizvod.SifraProizvoda == sifra))  // Provera proizvoda u stavkama
                .SelectMany(p => p.PrijemnicaStavke)  // Raspakujemo stavke
                .Where(stavka => stavka.Proizvod.SifraProizvoda == sifra)  // Filtriranje stavki po šifri proizvoda
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)  // Grupisanje po šifri proizvoda
                .Select(g => new
                {
                    UkupnaUlaznaVrednost = g.Sum(stavka => stavka.UlaznaVrednost),
                    UkupnaUlaznaKolicina = g.Sum(stavka => stavka.UlaznaKolicina)
                })
                .FirstOrDefaultAsync();

            // Upit za Kalkulacije
            var kalkulacije = await userDbContext.Kalkulacija
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                            && p.KalkulacijaStavke.Any(x => x.Proizvod.SifraProizvoda == sifra))  // Provera proizvoda u stavkama
                .SelectMany(p => p.KalkulacijaStavke)  // Raspakujemo stavke
                .Where(stavka => stavka.Proizvod.SifraProizvoda == sifra)  // Filtriranje stavki po šifri proizvoda
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)  // Grupisanje po šifri proizvoda
                .Select(g => new
                {
                    UkupnaUlaznaVrednost = g.Sum(stavka => stavka.UlaznaVrednost),
                    UkupnaUlaznaKolicina = g.Sum(stavka => stavka.UlaznaKolicina)
                })
                .FirstOrDefaultAsync();

            // Upit za Otpremnice
            var otpremnice = await userDbContext.Otpremnica
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                            && p.OtpremnicaStavke.Any(x => x.Proizvod.SifraProizvoda == sifra))  // Provera proizvoda u stavkama
                .SelectMany(p => p.OtpremnicaStavke)  // Raspakujemo stavke
                .Where(stavka => stavka.Proizvod.SifraProizvoda == sifra)  // Filtriranje stavki po šifri proizvoda
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)  // Grupisanje po šifri proizvoda
                .Select(g => new
                {
                    UkupnaIzlaznaVrednost = g.Sum(stavka => stavka.IzlaznaVrednost),
                    UkupnaIzlaznaKolicina = g.Sum(stavka => stavka.IzlaznaKolicina)
                })
                .FirstOrDefaultAsync();

            // Upit za Racune
            var racuni = await userDbContext.Racun
                 .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                             && p.RacunStavke.Any(x => x.Proizvod.SifraProizvoda == sifra))  // Provera proizvoda u stavkama
                 .SelectMany(p => p.RacunStavke)  // Raspakujemo stavke
                 .Where(stavka => stavka.Proizvod.SifraProizvoda == sifra)  // Filtriranje stavki po šifri proizvoda
                 .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)  // Grupisanje po šifri proizvoda
                 .Select(g => new
                 {
                     UkupnaIzlaznaVrednost = g.Sum(stavka => stavka.IzlaznaVrednost),
                     UkupnaIzlaznaKolicina = g.Sum(stavka => stavka.IzlaznaKolicina)
                 })
                 .FirstOrDefaultAsync();

            // Kombinacija rezultata iz svih tabela


            var ukupnaUlaznaVrednost =
                (prijemnice?.UkupnaUlaznaVrednost ?? 0) +
                (kalkulacije?.UkupnaUlaznaVrednost ?? 0);

            var ukupnaUlaznaKolicina =
                (prijemnice?.UkupnaUlaznaKolicina ?? 0) +
                (kalkulacije?.UkupnaUlaznaKolicina ?? 0);

            var ukupnaIzlaznaVrednost =
                (otpremnice?.UkupnaIzlaznaVrednost ?? 0) +
                (racuni?.UkupnaIzlaznaVrednost ?? 0);

            var ukupnaIzlaznaKolicina =
                (otpremnice?.UkupnaIzlaznaKolicina ?? 0) +
                (racuni?.UkupnaIzlaznaKolicina ?? 0);

            // Konačni proračun
            var razlikaVrednosti = ukupnaUlaznaVrednost - ukupnaIzlaznaVrednost;
            var razlikaKolicina = ukupnaUlaznaKolicina - ukupnaIzlaznaKolicina;
            var prosecnaVrednost = razlikaKolicina == 0 ? (decimal?)null : razlikaVrednosti / razlikaKolicina;

            var result = new
            {
                UkupnaUlaznaVrednost = ukupnaUlaznaVrednost,
                UkupnaIzlaznaVrednost = ukupnaIzlaznaVrednost,
                RazlikaVrednosti = razlikaVrednosti,
                UkupnaUlaznaKolicina = ukupnaUlaznaKolicina,
                UkupnaIzlaznaKolicina = ukupnaIzlaznaKolicina,
                RazlikaKolicina = razlikaKolicina,
                ProsecnaVrednost = prosecnaVrednost
            };

            var pr = await userDbContext.Proizvod.FirstOrDefaultAsync(x => x.SifraProizvoda == sifra);

            var proizvod = new
            {
                CurrentTime = DateTime.UtcNow,
                ImeProizvoda = pr.NazivProizvoda,
                SifraProizvoda = pr.SifraProizvoda,
                JM = pr.JM
            };

            return Ok(new { Izvestaj = result, Proizvod = proizvod });
        }
    }
}
