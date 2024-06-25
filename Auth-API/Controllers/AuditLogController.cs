using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public AuditLogsController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/AuditLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs()
        {
            return await _context.AuditLogs.Include(a => a.User).ToListAsync();
        }

        // GET: api/AuditLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLog>> GetAuditLog(Guid id)
        {
            var auditLog = await _context.AuditLogs.Include(a => a.User).FirstOrDefaultAsync(a => a.AuditLogId == id);

            if (auditLog == null)
            {
                return NotFound();
            }

            return auditLog;
        }

        // PUT: api/AuditLogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuditLog(Guid id, AuditLog auditLog)
        {
            if (id != auditLog.AuditLogId)
            {
                return BadRequest();
            }

            // Ensure User is not null
            if (auditLog.UserId != null)
            {
                auditLog.User = await _context.Users.FindAsync(auditLog.UserId);
            }

            _context.Entry(auditLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditLogExists(id))
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

        // POST: api/AuditLogs
        [HttpPost]
        public async Task<ActionResult<AuditLog>> PostAuditLog(AuditLog auditLog)
        {
            // Ensure User is not null
            if (auditLog.UserId != null)
            {
                auditLog.User = await _context.Users.FindAsync(auditLog.UserId);
            }

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuditLog", new { id = auditLog.AuditLogId }, auditLog);
        }

        // DELETE: api/AuditLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditLog(Guid id)
        {
            var auditLog = await _context.AuditLogs.FindAsync(id);
            if (auditLog == null)
            {
                return NotFound();
            }

            _context.AuditLogs.Remove(auditLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuditLogExists(Guid id)
        {
            return _context.AuditLogs.Any(e => e.AuditLogId == id);
        }
    }
}
