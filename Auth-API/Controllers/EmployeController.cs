﻿using Microsoft.AspNetCore.Mvc;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Repositories.Employe_repository;
using AutoMapper;
using ADMitroSremEmploye.Models.DTOs.Filters;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IEmployeRepository employeRepository;

        public EmployeController(IMapper mapper, IEmployeRepository employeRepository)
        {
            this.mapper = mapper;
            this.employeRepository = employeRepository;
        }

        // GET: api/Employe/get-employes
        [HttpGet("get-employes")]
        public async Task<ActionResult<IEnumerable<EmployeDto>>> GetEmployes(
            [FromQuery] EmployeFilterDto filterDto,
            [FromQuery] CommonFilterDto commonFilterDto)
        {
            var (totalCount, employes) = await employeRepository.GetEmployesAsync(filterDto, commonFilterDto);

            return Ok(new {TotalCount = totalCount, Employes = mapper.Map<IEnumerable<EmployeDto>>(employes)});
        }

        // GET: api/Employe/get-employe/id
        [HttpGet("get-employe/{id}")]
        public async Task<ActionResult<Employe?>> GetEmploye(Guid id)
        {
            var employe = await employeRepository.GetEmployeAsync(id);

            if (employe == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<EmployeDto>(employe));
        }

        // PUT: api/Employe/update-employe/id
        [HttpPut("update-employe/{id}")]
        public async Task<IActionResult?> UpdateEmploye(Guid id, EmployeDto employeDto)
        {
            if (id != employeDto.Id)
            {
                return BadRequest();
            }

            var employeDomain = mapper.Map<Employe>(employeDto);

            var updatedEmploye = await employeRepository.UpdateEmployeAsync(employeDomain);

            if (updatedEmploye == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<EmployeDto>(updatedEmploye));
        }

        // POST: api/Employe/create-employe
        [HttpPost("create-employe")]
        public async Task<ActionResult<EmployeDto>> CreateEmploye(EmployeDto employeDto)
        {
            var employeDomain = mapper.Map<Employe>(employeDto);

            var createdEmploye = await employeRepository.CreateEmployeAsync(employeDomain);

            var employeResponse = mapper.Map<EmployeDto>(createdEmploye);

            return CreatedAtAction("GetEmploye", new { id = employeResponse.Id }, employeResponse);
        }

        // Delete: api/Employe/delete-employe/id
        [HttpDelete("delete-employe/{id}")]
        public async Task<IActionResult> DeleteEmploye(Guid id)
        {
            var deleted = await employeRepository.DeleteEmployeAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
