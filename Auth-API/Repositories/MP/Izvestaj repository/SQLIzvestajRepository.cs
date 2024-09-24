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
    }
}
