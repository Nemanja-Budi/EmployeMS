using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Audit_repository;
using ADMitroSremEmploye.Repositories.Employe_repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuditRepository auditRepository;

        public AuditLogsController(IMapper mapper, IAuditRepository auditRepository)
        {
            this.mapper = mapper;
            this.auditRepository = auditRepository;
        }

        [HttpGet("get-auditlogs")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs(
            [FromQuery] AuditLogFilterDto filterDto,
            [FromQuery] CommonFilterDto commonFilterDto)
        {
            var (auditLogs, totalCount) = await auditRepository.GetAuditLogsAsync(filterDto, commonFilterDto);

            return Ok(new { TotalCount = totalCount, AuditLogs = mapper.Map<IEnumerable<AuditLogDto>>(auditLogs) });
        }

        // GET: api/auditlogs/get-auditlog/id
        [HttpGet("get-auditlog/{id}")]
        public async Task<ActionResult<AuditLogDto>> GetAuditLog(Guid id)
        {
            var auditLogDomain = await auditRepository.GetAuditLogAsync(id);

            if (auditLogDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AuditLogDto>(auditLogDomain));
        }

        // PUT: api/AuditLogs/update-auditlog/id
        [HttpPut("update-auditlog/{id}")]
        public async Task<IActionResult> PutAuditLog(Guid id, AuditLogDto auditLogDto)
        {

            var auditLog = mapper.Map<AuditLog>(auditLogDto);

            bool isUpdated = await auditRepository.UpdateAuditLogAsync(id, auditLog);

            if (!isUpdated)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/AuditLogs/create-auditlog
        [HttpPost("create-auditlog")]
        public async Task<ActionResult<AuditLogDto?>> PostAuditLog(AuditLogDto auditLogDto)
        {
            var auditLogDomain = mapper.Map<AuditLog>(auditLogDto);

            var createdAuditLog = await auditRepository.CreateAuditLogAsync(auditLogDomain);

            if (createdAuditLog == null)
            {
                return BadRequest("User not found or another error occurred."); // Prilagodite poruku prema potrebi
            }

            var respondeAuditLog = mapper.Map<AuditLogDto>(createdAuditLog);

            return CreatedAtAction("GetAuditLog", new { id = respondeAuditLog.AuditLogId }, respondeAuditLog);
        }

        // DELETE: api/AuditLogs/delete-auditlog
        [HttpDelete("delete-auditlog/{id}")]
        public async Task<IActionResult> DeleteAuditLog(Guid id)
        {
            var result = await auditRepository.DeleteAuditLogAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
