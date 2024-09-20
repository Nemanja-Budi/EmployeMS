using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz;
using ADMitroSremEmploye.Models.Domain.MP;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija;
using ADMitroSremEmploye.Models.Domain.MP.Ulaz.Prijemnica;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADMitroSremEmploye.Repositories.MP.Kalkulacija_repository;

namespace ADMitroSremEmploye.Controllers.MP
{
    [Route("api/mp/[controller]")]
    [ApiController]
    public class KalkulacijaController : ControllerBase
    {
        private readonly UserDbContext userDbContext;
        private readonly IKalkulacijaRepository kalkulacijaRepository;

        public KalkulacijaController(UserDbContext userDbContext, IKalkulacijaRepository kalkulacijaRepository)
        {
            this.userDbContext = userDbContext;
            this.kalkulacijaRepository = kalkulacijaRepository;
        }

        [HttpGet("get-kalkulacije")]
        public async Task<ActionResult<IEnumerable<Kalkulacija>>> GetKalkulacije()
        {
            var kalkulacije = await kalkulacijaRepository.GetKalkulacijeAsync();

            return Ok(kalkulacije);
        }

        [HttpPost("create-kalkulacija")]
        public async Task<ActionResult<Kalkulacija>> CreateKalkulacija(UlazCreate ulazCreate)
        {
            var newKalkulacija = await kalkulacijaRepository.CreateKalkulacijaAsync(ulazCreate);

            return Ok(newKalkulacija);
            
        }

        [HttpDelete("delete-kalkulacija/{id}")]
        public async Task<ActionResult> DeleteKalkulacija(Guid id)
        {
            var kalkulacija = await userDbContext.Kalkulacija
                .Include(k => k.KalkulacijaStavke)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kalkulacija == null)
            {
                return NotFound();
            }

            var dokumentId = kalkulacija.DokumentId;
            var dokument = await userDbContext.Dokument
                .FirstOrDefaultAsync(d => d.Id == dokumentId);

            if (dokument != null)
            {
                var prijemnica = await userDbContext.Prijemnica
                    .Include(p => p.PrijemnicaStavke)
                    .FirstOrDefaultAsync(p => p.DokumentId == dokumentId);

                if (prijemnica != null)
                {
                    userDbContext.PrijemnicaStavke.RemoveRange(prijemnica.PrijemnicaStavke);
                    userDbContext.Prijemnica.Remove(prijemnica);
                }


                userDbContext.KalkulacijaStavke.RemoveRange(kalkulacija.KalkulacijaStavke);
                userDbContext.Kalkulacija.Remove(kalkulacija);
                userDbContext.Dokument.Remove(dokument);
            }

            await userDbContext.SaveChangesAsync();

            return NoContent(); // Vrati HTTP 204 No Content
        }
    }
}
