using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Prijemnica;
using ADMitroSremEmploye.Models.Domain.MP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MpController : ControllerBase
    {
        private readonly UserDbContext userDbContext;

        public MpController(UserDbContext userDbContext)
        {
            this.userDbContext = userDbContext;
        }

        [HttpPost("create-racun")]
        public async Task<ActionResult<Racun>> CreateRacun(IzlazCreateRacun izlazCreateRacun)
        {
            var brojDokumenta = await userDbContext.Racun
              .OrderByDescending(k => k.Dokument.BrojDokumenta)
              .Select(k => k.Dokument.BrojDokumenta)
              .FirstOrDefaultAsync();


            var newDokument = new Dokument("RAC", brojDokumenta) { NazivDokumenta = "RAC" };
            userDbContext.Dokument.Add(newDokument);
            await userDbContext.SaveChangesAsync();

            var racunStavke = new List<RacunStavke>();


            foreach (var proizvod in izlazCreateRacun.Proizvodi)
            {
                var currentProizvod = await userDbContext.Proizvod.FirstOrDefaultAsync(x => x.Id == proizvod.ProizvodId);
                int pdv = currentProizvod.PoreskaGrupa == 4 ? 10 : 20;
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
            }

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

            userDbContext.Proizvod.UpdateRange(userDbContext.Proizvod);

            await userDbContext.SaveChangesAsync();

            return Ok(newRacun);
        }
    }
}

