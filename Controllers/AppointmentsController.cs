using HealthcareClaimsApi.Data;
using HealthcareClaimsApi.Dtos;
using HealthcareClaimsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareClaimsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .ToListAsync();
    }

    // GET: api/appointments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Claim)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return NotFound();
        return appointment;
    }

    // POST: api/appointments
    [HttpPost]
    public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentDto dto)
    {
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists) return BadRequest("PatientId does not exist.");

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            ScheduledAt = dto.ScheduledAt,
            ProviderName = dto.ProviderName,
            ReasonForVisit = dto.ReasonForVisit
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    // PATCH: api/appointments/5/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] AppointmentStatus status)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();

        appointment.Status = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
