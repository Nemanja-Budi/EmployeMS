using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeController : ControllerBase
    {
        private readonly UserDbContext _context;

        public EmployeController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/Employe
        [HttpGet("get-employes")]
        public async Task<ActionResult<IEnumerable<Employe>>> GetEmployes()
        {
            return await _context.Employe.ToListAsync();
        }

        // GET: api/Employe/5
        [HttpGet("get-employe/{id}")]
        public async Task<ActionResult<Employe>> GetEmploye(Guid id)
        {
            var employe = await _context.Employe.FindAsync(id);

            if (employe == null)
            {
                return NotFound();
            }

            return employe;
        }

        // PUT: api/Employe/5
        [HttpPut("update-employe/{id}")]
        public async Task<IActionResult> PutEmploye(Guid id, Employe employe)
        {
            if (id != employe.Id)
            {
                return BadRequest();
            }

            _context.Entry(employe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeExists(id))
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

        // POST: api/Employe
        [HttpPost("create-employe")]
        public async Task<ActionResult<Employe>> PostEmploye(Employe employe)
        {
            _context.Employe.Add(employe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmploye", new { id = employe.Id }, employe);
        }

        [HttpDelete("delete-employe/{id}")]
        public async Task<IActionResult> DeleteEmploye(Guid id)
        {
            // Pronađite zapise o zaposlenom uključujući povezane zapise dece
            var employe = await _context.Employe
                .Include(e => e.EmployeChild)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employe == null)
            {
                return NotFound();
            }

            // Pronađite sve povezane zapise dece
            var employeChildren = await _context.EmployeChild
                .Where(c => c.Id == id)
                .ToListAsync();

            // Obrisati sve zapise dece
            _context.EmployeChild.RemoveRange(employeChildren);

            // Obrisati zapis o zaposlenom
            _context.Employe.Remove(employe);

            // Sačuvati promene u bazi podataka
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeExists(Guid id)
        {
            return _context.Employe.Any(e => e.Id == id);
        }
    }
}
