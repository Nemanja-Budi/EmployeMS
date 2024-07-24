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
using ADMitroSremEmploye.Models.DTOs;
using AutoMapper;
using ADMitroSremEmploye.Repositories.Employe_Salary_repository;
using ADMitroSremEmploye.Repositories.Employe_repository;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeSalaryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly SalaryCalculatorService _salaryCalculatorService;
        private readonly IEmployeSalaryRepository employeSalaryRepository;

        public EmployeSalaryController(IMapper mapper, SalaryCalculatorService salaryCalculatorService, IEmployeSalaryRepository employeSalaryRepository)
        {
            this.mapper = mapper;
            _salaryCalculatorService = salaryCalculatorService;
            this.employeSalaryRepository = employeSalaryRepository;
        }

        [HttpPost("create-employe-salary")]
        public async Task<IActionResult> CalculateSalary([FromBody] EmployeSalaryDto employeSalaryDto)
        {
            var employeSalary = mapper.Map<EmployeSalary>(employeSalaryDto);

            if (employeSalary == null || employeSalary.EmployeId == Guid.Empty)
            {
                return BadRequest("Invalid employe salary data. EmployeId is required.");
            }

            var calculatedSalary = await _salaryCalculatorService.CalculateSalary(employeSalary);

            if (calculatedSalary == null)
            {
                return NotFound($"Employee with Id {employeSalary.EmployeId} not found.");
            }

            return Ok(mapper.Map<EmployeSalaryDto>(calculatedSalary));
        }

        [HttpGet("employe-salarys")]
        public async Task<IActionResult> GetAllEmployeSalarys([FromQuery] EmployeSalaryFilterDto filterDto, [FromQuery] string? sortBy, [FromQuery] bool isAscending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var employeSalarys = await employeSalaryRepository.GetAllEmployeSalarysAsync(filterDto, sortBy, isAscending, pageNumber, pageSize);
            var totalEmployesCount = await employeSalaryRepository.GetTotalEmployeSalariesCountAsync(filterDto);

            /*if (employeSalarys == null || !employeSalarys.Any())
            {
                return NotFound("No employee salaries found with the specified criteria.");
            }
            */
            return Ok(new { TotalCount = totalEmployesCount, EmployeSalarys = mapper.Map<IEnumerable<EmployeSalaryDto>>(employeSalarys) });

        }

        [HttpGet("employe-salarys/{employeId}")]
        public async Task<IActionResult> GetEmployeSalarys(Guid employeId)
        {
            var employeSalarys = await _salaryCalculatorService.GetEmployeSalarys(employeId);

            if (employeSalarys == null)
            {
                return NotFound($"Employee with Id {employeId} not found.");
            }

            return Ok(mapper.Map<List<EmployeSalaryDto>>(employeSalarys)); 
        }

        [HttpGet("employe-salary/{employesalaryId}")]
        public async Task<IActionResult> GetEmployeSalary(Guid employesalaryId)
        {
            var employeSalary = await _salaryCalculatorService.GetEmployeSalary(employesalaryId);

            if (employeSalary == null)
            {
                return NotFound($"Employee salary with Id {employesalaryId} not found.");
            }

            return Ok(mapper.Map<EmployeSalaryDto>(employeSalary));
        }

        [HttpDelete("delete-employe-salarys/{employeId}")]
        public async Task<IActionResult> DeleteEmployeSalarys(Guid employeId)
        {
            var result = await _salaryCalculatorService.DeleteEmployeSalarys(employeId);

            if (!result)
            {
                return NotFound($"Employee with Id {employeId} not found or no salaries found.");
            }

            return Ok($"Successfully deleted salaries for Employee with Id {employeId}.");
        }

        [HttpDelete("delete-employe-salary/{employeSalaryId}")]
        public async Task<IActionResult> DeleteEmployeSalary(Guid employeSalaryId)
        {
            var result = await _salaryCalculatorService.DeleteEmployeSalary(employeSalaryId);

            if (!result)
            {
                return NotFound($"EmployeSalary with Id {employeSalaryId} not found.");
            }

            return Ok($"Successfully deleted EmployeSalary with Id {employeSalaryId}.");
        }

        [HttpPut("update-employe-salary")]
        public async Task<IActionResult> UpdateEmployeSalary([FromBody] EmployeSalaryDto employeSalaryDto)
        {
            var updatedSalary = mapper.Map<EmployeSalary>(employeSalaryDto);

            if (updatedSalary == null || updatedSalary.Id == Guid.Empty)
            {
                return BadRequest("Invalid employe salary data.");
            }

            var result = await _salaryCalculatorService.UpdateSalary(updatedSalary);

            if (result == null)
            {
                return NotFound($"Employee or salary not found for EmployeId {updatedSalary.EmployeId} and SalaryId {updatedSalary.Id}.");
            }

            return Ok(mapper.Map<EmployeSalaryDto>(result));
        }

    }
}
