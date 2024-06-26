﻿using Microsoft.AspNetCore.Mvc;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using AutoMapper;
using ADMitroSremEmploye.Repositories;

[Route("api/[controller]")]
[ApiController]
public class AnnualLeaveController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IAnnualLeaveRepository annualLeaveRepository;

    public AnnualLeaveController(IMapper mapper, IAnnualLeaveRepository annualLeaveRepository)
    {
        this.mapper = mapper;
        this.annualLeaveRepository = annualLeaveRepository;
    }

    // GET: api/AnnualLeave/get-annualleaves
    [HttpGet("get-annualleaves")]
    public async Task<ActionResult<IEnumerable<AnnualLeaveDto>>> GetAnnualLeaves()
    {
        var annualLeaves = await annualLeaveRepository.GetAnnualLeavesAsync();
        return Ok(mapper.Map<IEnumerable<AnnualLeaveDto>>(annualLeaves));
    }

    // GET: api/AnnualLeave/get-annualleave/{id}
    [HttpGet("get-annualleave/{id}")]
    public async Task<ActionResult<AnnualLeaveDto>> GetAnnualLeave(Guid id)
    {
        var annualLeave = await annualLeaveRepository.GetAnnualLeaveAsync(id);

        if (annualLeave == null)
        {
            return NotFound();
        }

        return Ok(mapper.Map<AnnualLeaveDto>(annualLeave));
    }

    // POST: api/AnnualLeave/create-annualleave
    [HttpPost("create-annualleave")]
    public async Task<ActionResult<AnnualLeaveDto>> PostAnnualLeave(AnnualLeaveDto annualLeaveDto)
    {
        var annualLeaveDomain = mapper.Map<AnnualLeave>(annualLeaveDto);
        var result = await annualLeaveRepository.PostAnnualLeaveAsync(annualLeaveDomain);

        if (result.Result is BadRequestObjectResult badRequest)
        {
            return BadRequest(badRequest.Value);
        }

        var annualLeave = (AnnualLeave)((OkObjectResult)result.Result).Value;
        var annualLeaveDtoResult = mapper.Map<AnnualLeaveDto>(annualLeave);

        return CreatedAtAction(nameof(GetAnnualLeave), new { id = annualLeaveDtoResult.AnnualLeaveId }, annualLeaveDtoResult);
    }


    // PUT: api/AnnualLeave/edit-annualleave/{id}
    [HttpPut("edit-annualleave/{id}")]
    public async Task<IActionResult> PutAnnualLeave(Guid id, AnnualLeaveDto annualLeaveDto)
    {
        var annualLeaveDomain = mapper.Map<AnnualLeave>(annualLeaveDto);

        var result = await annualLeaveRepository.PutAnnualLeaveAsync(id, annualLeaveDomain);

        if (!result)
        {
            if (!annualLeaveRepository.AnnualLeaveExistsAsync(id))
            {
                return NotFound();
            }
            else
            {
                return BadRequest();
            }
        }

        return NoContent();
    }

    // DELETE: api/AnnualLeave/delete-annualleave/{id}
    [HttpDelete("delete-annualleave/{id}")]
    public async Task<IActionResult> DeleteAnnualLeave(Guid id)
    {
        var result = await annualLeaveRepository.DeleteAnnualLeaveAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

}
