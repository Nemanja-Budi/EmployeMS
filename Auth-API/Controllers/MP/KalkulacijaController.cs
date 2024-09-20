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
        private readonly IKalkulacijaRepository kalkulacijaRepository;

        public KalkulacijaController(IKalkulacijaRepository kalkulacijaRepository)
        {
            this.kalkulacijaRepository = kalkulacijaRepository;
        }

        [HttpGet("get-kalkulacije")]
        public async Task<ActionResult<IEnumerable<Kalkulacija>>> GetKalkulacije()
        {
            var kalkulacije = await kalkulacijaRepository.GetKalkulacijeAsync();

            return Ok(kalkulacije);
        }

        [HttpGet("get-kalkulacija/{id}")]
        public async Task<ActionResult<IEnumerable<Kalkulacija>>> GetKalkulacijaById(Guid id)
        {
            var kalkulacija = await kalkulacijaRepository.GetKalkulacijaByIdAsync(id);

            if (kalkulacija == null) return NotFound($"Kalkulacija sa id-em {id} nije pronadjena"); 

            return Ok(kalkulacija);
        }

        [HttpGet("get-kalkulacija-document/{documentId}")]
        public async Task<ActionResult<IEnumerable<Kalkulacija>>> GetKalkulacijaByDocumentId(Guid documentId)
        {
            var kalkulacija = await kalkulacijaRepository.GetKalkulacijaByDocumentIdAsync(documentId);

            if (kalkulacija == null) return NotFound($"Kalkulacija sa id-em {documentId} nije pronadjena");

            return Ok(kalkulacija);
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
            var kalkulacija = await kalkulacijaRepository.DeleteKalkulacijaAsync(id);

            if (kalkulacija == false)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
