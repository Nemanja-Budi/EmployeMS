using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Prijemnica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.MP.Kalkulacija_repository
{
    public class SQLKalkulacijaRepository : IKalkulacijaRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLKalkulacijaRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        public async Task<IEnumerable<Kalkulacija>> GetKalkulacijeAsync()
        {
            var kalkulacije = await userDbContext.Kalkulacija
               .Include(k => k.KalkulacijaStavke)
                   .ThenInclude(ks => ks.Proizvod)
               .Include(k => k.Dokument)
               .Include(k => k.Komintent)
               .ToListAsync();

            return kalkulacije;
        }

        public async Task<Kalkulacija> CreateKalkulacijaAsync(UlazCreate ulazCreate)
        {
            var brojDokumenta = await userDbContext.Kalkulacija
             .OrderByDescending(k => k.Dokument!.BrojDokumenta)
             .Select(k => k.Dokument!.BrojDokumenta)
             .FirstOrDefaultAsync();


            var newDokument = new Dokument("KAL", brojDokumenta) { NazivDokumenta = "KAL" };

            userDbContext.Dokument.Add(newDokument);
            await userDbContext.SaveChangesAsync();

            var kalkulacijeStavke = new List<KalkulacijaStavke>();
            var prijemnicaStavke = new List<PrijemnicaStavke>();

            foreach (var proizvod in ulazCreate.Proizvodi)
            {
                var currentProizvod = await userDbContext.Proizvod.FirstOrDefaultAsync(x => x.Id == proizvod.ProizvodId);

                decimal ulaznaVrednost = currentProizvod.CenaProizvoda * proizvod.UlaznaKolicina;
                int pdv = currentProizvod.PoreskaGrupa == 4 ? 10 : 20;
                decimal ulaznaCena = proizvod.UlaznaCena;
                decimal ulaznaKolicina = proizvod.UlaznaKolicina;
                decimal nabavnaVrednost = ulaznaCena * ulaznaKolicina;
                decimal pdvUDin = nabavnaVrednost * (pdv == 10 ? 0.1m : 0.2m);
                decimal vrednostRobeSaPdv = nabavnaVrednost + pdvUDin;
                decimal cenaProizvodaBezPdv = ulaznaCena;
                decimal cenaProizvodaSaPdv = (cenaProizvodaBezPdv * (pdv == 10 ? 1.1m : 1.2m));

                var newKalkulacijaStavke = new KalkulacijaStavke
                {
                    UlaznaKolicina = ulaznaKolicina,
                    UlaznaVrednost = ulaznaVrednost,
                    Kolicina = ulaznaKolicina,
                    UlaznaCena = ulaznaCena,
                    PDV = pdv,
                    NabavnaCena = ulaznaCena,
                    NabavnaVrednost = nabavnaVrednost,
                    VrednostRobeBezPdv = nabavnaVrednost,
                    PdvUDin = pdvUDin,
                    VrednostRobeSaPdv = vrednostRobeSaPdv,
                    CenaProizvodaBezPdv = cenaProizvodaBezPdv,
                    CenaProizvodaSaPdv = cenaProizvodaSaPdv,
                    ProizvodId = currentProizvod.Id,
                };
                kalkulacijeStavke.Add(newKalkulacijaStavke);

                var newPrijemnicaStavke = new PrijemnicaStavke
                {
                    UlaznaKolicina = ulaznaKolicina,
                    UlaznaVrednost = ulaznaVrednost,
                    ProizvodId = currentProizvod.Id,
                };
                prijemnicaStavke.Add(newPrijemnicaStavke);

                currentProizvod.ZaliheProizvoda += ulaznaKolicina;
            }

            var newKalkulacija = new Kalkulacija
            {
                Id = Guid.NewGuid(),
                DokumentId = newDokument.Id,
                KomintentId = ulazCreate.KomintentiId,
                KalkulacijaStavke = kalkulacijeStavke
            };

            var newPrijemnnica = new Prijemnica
            {
                Id = Guid.NewGuid(),
                DokumentId = newDokument.Id,
                KomintentId = ulazCreate.KomintentiId,
                PrijemnicaStavke = prijemnicaStavke
            };


            userDbContext.Kalkulacija.AddRange(newKalkulacija);
            userDbContext.Prijemnica.AddRange(newPrijemnnica);

            userDbContext.Proizvod.UpdateRange(userDbContext.Proizvod);

            await userDbContext.SaveChangesAsync();

            return newKalkulacija;
        }

        public async Task DeleteKalkulacijaAsync(Guid id)
        {
            var kalkulacija = await userDbContext.Kalkulacija
                .Include(k => k.KalkulacijaStavke)
                .ThenInclude(ks => ks.Proizvod)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kalkulacija == null) return;

            userDbContext.KalkulacijaStavke.RemoveRange(kalkulacija.KalkulacijaStavke);
            userDbContext.Kalkulacija.Remove(kalkulacija);

            var dokument = await userDbContext.Dokument
                .FirstOrDefaultAsync(d => d.Id == kalkulacija.DokumentId);

            if (dokument == null) return;
                
            var prijemnica = await userDbContext.Prijemnica
                .Include(p => p.PrijemnicaStavke)
                .FirstOrDefaultAsync(p => p.DokumentId == dokument.Id);

            if (prijemnica == null) return;

            userDbContext.PrijemnicaStavke.RemoveRange(prijemnica.PrijemnicaStavke);
            userDbContext.Prijemnica.Remove(prijemnica);
            userDbContext.Dokument.Remove(dokument);
        }
    }
}
