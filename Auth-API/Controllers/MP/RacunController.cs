using ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun;
using ADMitroSremEmploye.Repositories.MP.Racun_repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Controllers.MP
{
    [Route("api/mp/[controller]")]
    [ApiController]
    public class RacunController : ControllerBase
    {
        private readonly IRacunRepository racunRepository;

        public RacunController(IRacunRepository racunRepository)
        {
            this.racunRepository = racunRepository;
        }

        [HttpPost("create-racun")]
        public async Task<ActionResult<Racun>> CreateRacun(IzlazCreateRacun izlazCreateRacun)
        {
            var newRacun = await racunRepository.CreateRacunAsync(izlazCreateRacun);

            return Ok(newRacun);
        }
    }
}
