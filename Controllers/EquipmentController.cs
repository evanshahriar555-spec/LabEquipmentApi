using LabEquipmentApi.Contracts.Equipment;
using LabEquipmentApi.Data;
using Mapster;
using LabEquipmentApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabEquipmentApi.Controllers;

[Route("api/equipment")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly LabDbContext _context;

    public EquipmentController(LabDbContext context)
    {
        _context = context;
    }

    // GET: api/equipment
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipment()
    {
        return await _context.EquipmentItems.AsNoTracking().ToListAsync();
    }

    // GET: api/equipment/available (response DTO, sorted by name)
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<EquipmentListItemResponse>>> GetAvailableEquipment()
    {
        var available = await _context.EquipmentItems
            .AsNoTracking()
            .Where(e => e.IsAvailable)
            .OrderBy(e => e.Name)
            .ProjectToType<EquipmentListItemResponse>()
            .ToListAsync();

        return Ok(available);
    }

    // GET: api/equipment/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Equipment>> GetEquipment(int id)
    {
        var equipment = await _context.EquipmentItems.FindAsync(id);
        if (equipment is null)
        {
            return NotFound();
        }
        return equipment;
    }

    // PUT: api/equipment/5 (scaffold-style full update)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutEquipment(int id, Equipment equipment)
    {
        if (id != equipment.Id)
        {
            return BadRequest();
        }

        _context.Entry(equipment).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.EquipmentItems.AnyAsync(x => x.Id == id))
            {
                return NotFound();
            }
            throw;
        }
        return NoContent();
    }

    // POST: api/equipment (will be replaced with a validated DTO in Part 5)
    [HttpPost]
    public async Task<ActionResult<Equipment>> PostEquipment(Equipment equipment)
    {
        _context.EquipmentItems.Add(equipment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEquipment), new { id = equipment.Id }, equipment);
    }

    // DELETE: api/equipment/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEquipment(int id)
    {
        var equipment = await _context.EquipmentItems.FindAsync(id);
        if (equipment is null)
        {
            return NotFound();
        }
        _context.EquipmentItems.Remove(equipment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
