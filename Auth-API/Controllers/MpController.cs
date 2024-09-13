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

        [HttpPost("create-kalkulacija")]
        public async Task<ActionResult<Kalkulacija>> CreateKalkulacija(UlazCreate ulazCreate)
        {

            var brojDokumenta = await userDbContext.Kalkulacija
             .OrderByDescending(k => k.Dokument.BrojDokumenta)
             .Select(k => k.Dokument.BrojDokumenta)
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

            return Ok(newKalkulacija);
        }

        [HttpGet("get-kalkulacije")]
        public async Task<ActionResult<IEnumerable<Kalkulacija>>> GetKalkulacije()
        {
            var kalkulacije = await userDbContext.Kalkulacija
                .Include(k => k.KalkulacijaStavke)
                    .ThenInclude(ks => ks.Proizvod) // Uključi proizvod
                .Include(k => k.Dokument)
                .Include(k => k.Komintent) // Uključi dokument
                .ToListAsync();

            return Ok(kalkulacije);
        }

        [HttpDelete("delete-kalkulacija/{id}")]
        public async Task<ActionResult> DeleteKalkulacija(Guid id)
        {
            // Pronađi Kalkulaciju s povezanim Stavkama
            var kalkulacija = await userDbContext.Kalkulacija
                .Include(k => k.KalkulacijaStavke)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kalkulacija == null)
            {
                return NotFound();
            }

            // Pronađi povezani Dokument
            var dokumentId = kalkulacija.DokumentId;
            var dokument = await userDbContext.Dokument
                .FirstOrDefaultAsync(d => d.Id == dokumentId);

            // Ako je pronađen, obriši sve povezane Kalkulacija i Prijemnica
            if (dokument != null)
            {
                // Ako postoji Prijemnica povezana sa dokumentom
                var prijemnica = await userDbContext.Prijemnica
                    .Include(p => p.PrijemnicaStavke)
                    .FirstOrDefaultAsync(p => p.DokumentId == dokumentId);

                if (prijemnica != null)
                {
                    // Obriši PrijemnicaStavke
                    userDbContext.PrijemnicaStavke.RemoveRange(prijemnica.PrijemnicaStavke);
                    // Obriši Prijemnicu
                    userDbContext.Prijemnica.Remove(prijemnica);
                }

                // Obriši KalkulacijaStavke
                userDbContext.KalkulacijaStavke.RemoveRange(kalkulacija.KalkulacijaStavke);
                // Obriši Kalkulaciju
                userDbContext.Kalkulacija.Remove(kalkulacija);

                // Obriši Dokument
                userDbContext.Dokument.Remove(dokument);
            }

            // Sačuvaj promjene u bazi podataka
            await userDbContext.SaveChangesAsync();

            return NoContent(); // Vrati HTTP 204 No Content
        }

    }
}

