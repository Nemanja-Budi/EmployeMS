using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeSalaryController : ControllerBase
    {
        private readonly SalaryCalculatorService _salaryCalculatorService;
        private readonly UserDbContext userDbContext;

        public EmployeSalaryController(SalaryCalculatorService salaryCalculatorService, UserDbContext userDbContext)
        {
            _salaryCalculatorService = salaryCalculatorService;
            this.userDbContext = userDbContext;
        }

        [HttpPost("create-employe-salary")]
        public IActionResult CalculateSalary([FromBody] EmployeSalary employeSalary)
        {
            if (employeSalary == null || employeSalary.EmployeId == Guid.Empty)
            {
                return BadRequest("Invalid employe salary data. EmployeId is required.");
            }

            var employe = userDbContext.Employe.FirstOrDefault(e => e.Id == employeSalary.EmployeId);
            if (employe == null)
            {
                return BadRequest($"Employee with Id {employeSalary.EmployeId} not found.");
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                string json = JsonSerializer.Serialize(employeSalary, options);
                EmployeSalary calculatedSalary = _salaryCalculatorService.CalculateSalary(employeSalary.EmployeId, employeSalary);
                return Ok(calculatedSalary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-employe-salary")]
        public ActionResult GetEmployeSalary()
        {
            var employeSalary = userDbContext.EmployeSalary.Include(e => e.EmployeSalarySO).Include(e => e.EmployeSalarySOE).ToList();

            return Ok(employeSalary);
        }

    }
}
