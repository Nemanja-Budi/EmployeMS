using Microsoft.AspNetCore.Mvc;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using AutoMapper;
using ADMitroSremEmploye.Repositories;
using ADMitroSremEmploye.Services;
using ADMitroSremEmploye.Models.DTOs.Filters;

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
    public async Task<ActionResult<IEnumerable<AnnualLeaveDto>>> GetAnnualLeaves(
        [FromQuery] AnnualLeaveFilterDto filterDto, 
        [FromQuery] string? sortBy, 
        [FromQuery] bool isAscending = true, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 1000)
    {
        var (totalCount, annualLeaves) = await annualLeaveRepository.GetAnnualLeavesAsync(filterDto, sortBy,isAscending,pageNumber,pageSize);
        //return Ok(mapper.Map<IEnumerable<AnnualLeaveDto>>(annualLeaves));
        return Ok(new { TotalCount = totalCount, AnnualLeaves = mapper.Map<IEnumerable<AnnualLeaveDto>>(annualLeaves) });
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
            return NotFound($"Employe Annual Leave with Id {id} not found or no salaries found.");
        }
        return Ok(new { message = $"Successfully deleted Employe Annual Leave with Id {id}." });
    }
}


