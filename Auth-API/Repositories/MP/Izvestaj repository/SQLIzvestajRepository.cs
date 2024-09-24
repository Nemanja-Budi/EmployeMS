using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.DTOs.MP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.MP.Izvestaj_repository
{
    public class SQLIzvestajRepository : IIzvestajRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLIzvestajRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<Izvestaj> GetIzvestajZaProizvodAsync([FromQuery] string sifra, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            var prijemnice = await userDbContext.Prijemnica
                .Where(p => p.Dokument!.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate && p.PrijemnicaStavke.Any(x => x.Proizvod!.SifraProizvoda == sifra))
                .SelectMany(p => p.PrijemnicaStavke) 
                .Where(stavka => stavka.Proizvod!.SifraProizvoda == sifra)
                .GroupBy(stavka => stavka.Proizvod!.SifraProizvoda)
                .Select(g => new
                {
                    UkupnaUlaznaVrednost = g.Sum(stavka => stavka.UlaznaVrednost),
                    UkupnaUlaznaKolicina = g.Sum(stavka => stavka.UlaznaKolicina)
                })
                .FirstOrDefaultAsync();

            var kalkulacije = await userDbContext.Kalkulacija
                .Where(p => p.Dokument!.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                            && p.KalkulacijaStavke.Any(x => x.Proizvod!.SifraProizvoda == sifra))
                .SelectMany(p => p.KalkulacijaStavke)
                .Where(stavka => stavka.Proizvod!.SifraProizvoda == sifra)
                .GroupBy(stavka => stavka.Proizvod!.SifraProizvoda) 
                .Select(g => new
                {
                    UkupnaUlaznaVrednost = g.Sum(stavka => stavka.UlaznaVrednost),
                    UkupnaUlaznaKolicina = g.Sum(stavka => stavka.UlaznaKolicina)
                })
                .FirstOrDefaultAsync();

            var otpremnice = await userDbContext.Otpremnica
                .Where(p => p.Dokument!.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                            && p.OtpremnicaStavke.Any(x => x.Proizvod!.SifraProizvoda == sifra))
                .SelectMany(p => p.OtpremnicaStavke)
                .Where(stavka => stavka.Proizvod!.SifraProizvoda == sifra)
                .GroupBy(stavka => stavka.Proizvod!.SifraProizvoda)
                .Select(g => new
                {
                    UkupnaIzlaznaVrednost = g.Sum(stavka => stavka.IzlaznaVrednost),
                    UkupnaIzlaznaKolicina = g.Sum(stavka => stavka.IzlaznaKolicina)
                })
                .FirstOrDefaultAsync();

            var racuni = await userDbContext.Racun
                 .Where(p => p.Dokument!.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate
                             && p.RacunStavke.Any(x => x.Proizvod!.SifraProizvoda == sifra))
                 .SelectMany(p => p.RacunStavke)
                 .Where(stavka => stavka.Proizvod!.SifraProizvoda == sifra)
                 .GroupBy(stavka => stavka.Proizvod!.SifraProizvoda)
                 .Select(g => new
                 {
                     UkupnaIzlaznaVrednost = g.Sum(stavka => stavka.IzlaznaVrednost),
                     UkupnaIzlaznaKolicina = g.Sum(stavka => stavka.IzlaznaKolicina)
                 })
                 .FirstOrDefaultAsync();

            var ukupnaUlaznaVrednost = (prijemnice?.UkupnaUlaznaVrednost ?? 0) + (kalkulacije?.UkupnaUlaznaVrednost ?? 0);
            var ukupnaUlaznaKolicina = (prijemnice?.UkupnaUlaznaKolicina ?? 0) + (kalkulacije?.UkupnaUlaznaKolicina ?? 0);
            var ukupnaIzlaznaVrednost = (otpremnice?.UkupnaIzlaznaVrednost ?? 0) + (racuni?.UkupnaIzlaznaVrednost ?? 0);
            var ukupnaIzlaznaKolicina = (otpremnice?.UkupnaIzlaznaKolicina ?? 0) + (racuni?.UkupnaIzlaznaKolicina ?? 0);

            var razlikaVrednosti = ukupnaUlaznaVrednost - ukupnaIzlaznaVrednost;
            var razlikaKolicina = ukupnaUlaznaKolicina - ukupnaIzlaznaKolicina;
            var prosecnaVrednost = razlikaKolicina == 0 ? (decimal?)null : razlikaVrednosti / razlikaKolicina;

            var proizvod = await userDbContext.Proizvod
               .Select(p => new ProizvodIzvestaj
               {
                   SifraProizvoda = p.SifraProizvoda,
                   NazivProizvoda = p.NazivProizvoda,
                   JM = p.JM
               })
               .FirstOrDefaultAsync(x => x.SifraProizvoda == sifra);

            var izvestaj = new Izvestaj
            {
                Proizvod = proizvod!,
                UkupnaUlaznaVrednost = ukupnaUlaznaVrednost,
                UkupnaIzlaznaVrednost = ukupnaIzlaznaVrednost,
                RazlikaVrednosti = razlikaVrednosti,
                UkupnaUlaznaKolicina = ukupnaUlaznaKolicina,
                UkupnaIzlaznaKolicina = ukupnaIzlaznaKolicina,
                RazlikaKolicina = razlikaKolicina,
                ProsecnaVrednost = prosecnaVrednost,
            };

            return izvestaj;
        }

        public async Task<List<Izvestaj>> GetIzvestajZaProizvodeAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            var prijemnice = await userDbContext.Prijemnica
                .AsNoTracking()
                .AsSplitQuery()
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

            var kalkulacije = await userDbContext.Kalkulacija
                .AsNoTracking()
                .AsSplitQuery()
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

            var otpremnice = await userDbContext.Otpremnica
                .AsNoTracking()
                .AsSplitQuery()
                .Where(p => p.Dokument.DatumDokumenta >= startDate && p.Dokument.DatumDokumenta <= adjustedEndDate)
                .Include(p => p.OtpremnicaStavke) // Eager Loading stavki
                .Select(p => new
                {
                    p.OtpremnicaStavke
                })
                .SelectMany(p => p.OtpremnicaStavke)
                .GroupBy(stavka => stavka.Proizvod.SifraProizvoda)
                .Select(g => new
                {
                    SifraProizvoda = g.Key,
                    UkupnaIzlaznaVrednost = g.Sum(stavka => (decimal?)stavka.IzlaznaVrednost) ?? 0,
                    UkupnaIzlaznaKolicina = g.Sum(stavka => (decimal?)stavka.IzlaznaKolicina) ?? 0
                })
                .ToListAsync();

            var racuni = await userDbContext.Racun
                .AsNoTracking()
                .AsSplitQuery()
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

            var listaProizvoda = await userDbContext.Proizvod
                .AsNoTracking()
                .AsSplitQuery()
                .Select(p => new ProizvodIzvestaj
                {
                    SifraProizvoda = p.SifraProizvoda,
                    NazivProizvoda = p.NazivProizvoda,
                    JM = p.JM
                })
                .ToListAsync();

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

            return spojeniRezultati;
        }
    }
}
