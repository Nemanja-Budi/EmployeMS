using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun;
using ADMitroSremEmploye.Models.Domain.MP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.MP.Racun_repository
{
    public class SQLRacunRepository : IRacunRepository
    {
        private readonly UserDbContext userDbContext;

        public SQLRacunRepository(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        private async Task<Dokument> CreateRacunDokument()
        {
            var brojDokumenta = await userDbContext.Racun
              .OrderByDescending(k => k.Dokument!.BrojDokumenta)
              .Select(k => k.Dokument!.BrojDokumenta)
              .FirstOrDefaultAsync();


            var newDokument = new Dokument("RAC", brojDokumenta) { NazivDokumenta = "RAC" };
            userDbContext.Dokument.Add(newDokument);
            await userDbContext.SaveChangesAsync();

            return newDokument;
        }

        private async Task<List<RacunStavke>> CreateRacunStavkeDokument(IzlazCreateRacun izlazCreateRacun)
        {
            var racunStavke = new List<RacunStavke>();

            foreach (var proizvod in izlazCreateRacun.Proizvodi)
            {
                var currentProizvod = await userDbContext.Proizvod.FirstOrDefaultAsync(x => x.Id == proizvod.ProizvodId);
                int pdv = currentProizvod!.PoreskaGrupa == 4 ? 10 : 20;
                decimal izlaznaVrednost = proizvod.IzlaznaKolicina * currentProizvod.CenaProizvoda;
                decimal pdvUDin = (izlaznaVrednost / (pdv == 10 ? 1.1m : 1.2m)) / 10;

                var newRacunStavke = new RacunStavke
                {
                    IzlaznaKolicina = proizvod.IzlaznaKolicina,
                    IzlaznaVrednost = izlaznaVrednost,
                    ProizvodId = currentProizvod.Id,
                    PDV = pdv,
                    CenaBezPdv = proizvod.IzlaznaCena,
                    PdvUDin = pdvUDin,
                };

                racunStavke.Add(newRacunStavke);

                currentProizvod.ZaliheProizvoda -= proizvod.IzlaznaKolicina;
                userDbContext.Proizvod.Update(currentProizvod);
            }

            return racunStavke;
        }

        public async Task<Racun> CreateRacunAsync(IzlazCreateRacun izlazCreateRacun)
        {

            var newDokument = await CreateRacunDokument();
            var racunStavke = await CreateRacunStavkeDokument(izlazCreateRacun);

            var newRacun = new Racun
            {
                Id = Guid.NewGuid(),
                RacunStavke = racunStavke,
                Paritet = izlazCreateRacun.Paritet,
                BrojFiskalnogRacuna = izlazCreateRacun.BrojFiskalnogRacuna,
                PIB = izlazCreateRacun.PIB,
                MaticniBroj = izlazCreateRacun.MaticniBroj,
                Primalac = izlazCreateRacun.Primalac,
                KomintentiId = izlazCreateRacun.KomintentiId,
                DokumentId = newDokument.Id,
            };

            userDbContext.Racun.AddRange(newRacun);

            await userDbContext.SaveChangesAsync();

            return newRacun;
        }
    }
}
