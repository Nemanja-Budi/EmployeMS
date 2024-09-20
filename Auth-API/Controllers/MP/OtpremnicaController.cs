using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Otpremnica;
using ADMitroSremEmploye.Repositories.MP.Otpremnica_repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Controllers.MP
{
    [Route("api/mp/[controller]")]
    [ApiController]
    public class OtpremnicaController : ControllerBase
    {
        private readonly IOtpremnicaRepository otpremnicaRepository;

        public OtpremnicaController(IOtpremnicaRepository otpremnicaRepository)
        {
            this.otpremnicaRepository = otpremnicaRepository;
        }

        [HttpGet("get-otpremnice")]
        public async Task<ActionResult<IEnumerable<Otpremnica>>> GetOtpremnice()
        {
            var otpremnice = await otpremnicaRepository.GetOtpremniceAsync();

            return Ok(otpremnice);
        }

        [HttpGet("get-otpremnica/{id}")]
        public async Task<ActionResult<IEnumerable<Otpremnica>>> GetOtpremnica(Guid id)
        {
            var otpremnica = await otpremnicaRepository.GetOtpremnicaByIdAsync(id);

            if (otpremnica == null) return NotFound($"Otpremnica sa id-em {id} nije pronadjena");

            return Ok(otpremnica);
        }
    }
}
