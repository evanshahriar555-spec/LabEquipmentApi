using LabEquipmentApi.Contracts.Equipment;
using LabEquipmentApi.Data;
using Mapster;
using LabEquipmentApi.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabEquipmentApi.Controllers;

[Route("api/equipment")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly LabDbContext _context;
    private readonly IValidator<CreateEquipmentRequest> _createValidator;

    public EquipmentController(LabDbContext context, IValidator<CreateEquipmentRequest> createValidator)
    {
        _context = context;
        _createValidator = createValidator;
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

    // Optional: GET /api/equipment/room-summary
    [HttpGet("room-summary")]
    public async Task<ActionResult<IEnumerable<RoomSummaryResponse>>> GetRoomSummary()
    {
        var summary = await _context.EquipmentItems
            .AsNoTracking()
            .GroupBy(e => e.Room)
            .Select(g => new RoomSummaryResponse
            {
                Room = g.Key,
                TotalCount = g.Count(),
                AvailableCount = g.Count(e => e.IsAvailable)
            })
            .OrderBy(x => x.Room)
            .ToListAsync();

        return Ok(summary);
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

    // POST: api/equipment — a safe request DTO plus FluentValidation
    [HttpPost]
    public async Task<ActionResult<Equipment>> PostEquipment(CreateEquipmentRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }

        var equipment = new Equipment
        {
            Name = request.Name,
            Room = request.Room,
            IsAvailable = true
        };

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
