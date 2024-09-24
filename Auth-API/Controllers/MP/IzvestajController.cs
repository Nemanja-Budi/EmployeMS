using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP;
using ADMitroSremEmploye.Models.DTOs.MP;
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
                kal = new { UK = kalkulacije?.UkupnaUlaznaKolicina ?? 0, UV = kalkulacije?.UkupnaUlaznaVrednost ?? 0 },
                pri = new { UK = prijemnice?.UkupnaUlaznaKolicina ?? 0, UV = prijemnice?.UkupnaUlaznaVrednost ?? 0 },
                otp = new { IK = otpremnice?.UkupnaIzlaznaKolicina ?? 0, IV = otpremnice?.UkupnaIzlaznaVrednost ?? 0 },
                rac = new { IK = racuni?.UkupnaIzlaznaKolicina ?? 0, IV = racuni?.UkupnaIzlaznaVrednost ?? 0},
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

        [HttpGet("izvestaj")]
        public async Task<IActionResult> GetAggregatedData2(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            // Adjust endDate
            var adjustedStartDate = startDate.Date.AddDays(1).AddTicks(-1);

            var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            // Upit za Prijemnice
            var prijemnice = await userDbContext.Prijemnica
                .Where(p => p.Dokument.DatumDokumenta >= adjustedStartDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
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
                .Where(p => p.Dokument.DatumDokumenta >= adjustedStartDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
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
                .Where(p => p.Dokument.DatumDokumenta >= adjustedStartDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
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
                .Where(p => p.Dokument.DatumDokumenta >= adjustedStartDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
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
                .Select(p => new
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
                (r, p) => new
                {
                    SifraProizvoda = r.SifraProizvoda,
                    NazivProizvoda = p.NazivProizvoda,
                    JedinicaMere = p.JM,
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
