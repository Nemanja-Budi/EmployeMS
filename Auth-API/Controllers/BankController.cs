using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Bank_repository;
using ADMitroSremEmploye.Repositories.Employe_repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankRepository bankRepository;
        private readonly IMapper mapper;

        public BankController(IBankRepository bankRepository, IMapper mapper)
        {
            this.bankRepository = bankRepository;
            this.mapper = mapper;
        }

        [HttpGet("get-banks")]
        public async Task<ActionResult<IEnumerable<BankDto>>> GetBanks() 
        {
            var banks = await bankRepository.GetBanksAsync();

            return Ok(mapper.Map<IEnumerable<BankDto>>(banks));
        }

        [HttpGet("get-bank/{id}")]
        public async Task<ActionResult<BankDto>> GetBank(Guid id)
        {
            var bank = await bankRepository.GetBankByIdAsync(id);
            
            if(bank == null)
            {
                return NotFound($"Bank with id ${id} not found");
            }

            return Ok(mapper.Map<BankDto>(bank));
        }

        [HttpPost("create-bank")]
        public async Task<ActionResult<BankDto>> CreateBank(BankDto bankDto)
        {
            var domainBank = mapper.Map<Bank>(bankDto);

            var bank = await bankRepository.CreateBankAsync(domainBank);

            return Ok(mapper.Map<BankDto>(bank));
        }

        [HttpPut("update-bank/{id}")]
        public async Task<ActionResult<BankDto>> UpdateBank(Guid id, BankDto bankDto)
        {
            var domainBank = mapper.Map<Bank>(bankDto);

            domainBank.Id = id;

            var existingBank = await bankRepository.UpdateBankAsync(domainBank);

            if(existingBank == null)
            {
                return NotFound($"Bank with id {id} not found");
            }

            return Ok(mapper.Map<BankDto>(existingBank));
        }

        [HttpDelete("delete-bank/{id}")]
        public async Task<IActionResult> DeleteBank(Guid id)
        {
            var deletedBank = await bankRepository.DeleteBankByIdAsync(id);

            if(deletedBank == false)
            {
                return NotFound($"Bank with id {id} not found");
            }

            return Ok(new { message = "Bank deleted successfully" });
        }
    }
}
