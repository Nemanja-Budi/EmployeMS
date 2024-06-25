using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using AutoMapper;
using ADMitroSremEmploye.Repositories;

[Route("api/[controller]")]
[ApiController]
public class AnnualLeaveController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper mapper;
    private readonly IAnnualLeaveRepository annualLeaveRepository;

    public AnnualLeaveController(UserDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAnnualLeaveRepository annualLeaveRepository)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        this.mapper = mapper;
        this.annualLeaveRepository = annualLeaveRepository;
    }

    [HttpGet("get-annualleave")]
    public async Task<ActionResult<IEnumerable<AnnualLeave>>> GetAnnualLeaves()
    {
        var annualLeaves = await annualLeaveRepository.GetAnnualLeavesAsync();
        return Ok(annualLeaves);
    }

    // GET: api/AnnualLeave/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AnnualLeave>> GetAnnualLeave(Guid id)
    {
        var annualLeave = await _context.AnnualLeaves.FindAsync(id);

        if (annualLeave == null)
        {
            return NotFound();
        }

        return annualLeave;
    }

    [HttpPost]
    public async Task<ActionResult<AnnualLeaveDto>> PostAnnualLeave(AnnualLeaveDto annualLeaveDto)
    {
        var annualLeaveDomain = mapper.Map<AnnualLeave>(annualLeaveDto);

        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var existingEmployee = await _context.Employe.FindAsync(annualLeaveDomain.EmployeId);
        if (existingEmployee == null)
        {
            return BadRequest("Zaposlenik sa datim ID-jem ne postoji.");
        }

        annualLeaveDomain.CreatedByUserId = userId;
        annualLeaveDomain.CreatedDate = DateTime.UtcNow;
        annualLeaveDomain.Employe = existingEmployee;

        _context.AnnualLeaves.Add(annualLeaveDomain);
        await _context.SaveChangesAsync();

        var AnnualLeaveDto = mapper.Map<AnnualLeaveDto>(annualLeaveDomain);

        return CreatedAtAction(nameof(GetAnnualLeave), new { id = AnnualLeaveDto.AnnualLeaveId }, AnnualLeaveDto);
    }


    // PUT: api/AnnualLeave/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnnualLeave(Guid id, AnnualLeave annualLeave)
    {
        if (id != annualLeave.AnnualLeaveId)
        {
            return BadRequest();
        }

        // Postavljamo informacije o korisniku koji je ažurirao unos
        annualLeave.CreatedByUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        annualLeave.CreatedDate = DateTime.UtcNow;

        _context.Entry(annualLeave).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnnualLeaveExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/AnnualLeave/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnnualLeave(Guid id)
    {
        var annualLeave = await _context.AnnualLeaves.FindAsync(id);
        if (annualLeave == null)
        {
            return NotFound();
        }

        _context.AnnualLeaves.Remove(annualLeave);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AnnualLeaveExists(Guid id)
    {
        return _context.AnnualLeaves.Any(e => e.AnnualLeaveId == id);
    }
}
