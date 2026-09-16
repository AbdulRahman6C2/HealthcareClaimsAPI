using HealthcareClaimsApi.Data;
using HealthcareClaimsApi.Dtos;
using HealthcareClaimsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareClaimsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClaimsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/claims?status=FlaggedForAudit
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Claim>>> GetClaims([FromQuery] ClaimStatus? status)
    {
        var query = _context.Claims.Include(c => c.Appointment).AsQueryable();
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }
        return await query.ToListAsync();
    }

    // GET: api/claims/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Claim>> GetClaim(int id)
    {
        var claim = await _context.Claims
            .Include(c => c.Appointment)
            .ThenInclude(a => a!.Patient)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (claim == null) return NotFound();
        return claim;
    }

    // POST: api/claims  — submitting a claim runs it through the initial review rule
    [HttpPost]
    public async Task<ActionResult<Claim>> SubmitClaim(CreateClaimDto dto)
    {
        var appointmentExists = await _context.Appointments.AnyAsync(a => a.Id == dto.AppointmentId);
        if (!appointmentExists) return BadRequest("AppointmentId does not exist.");

        var claim = new Claim
        {
            AppointmentId = dto.AppointmentId,
            BilledAmount = dto.BilledAmount,
            ProcedureCode = dto.ProcedureCode,
            Status = Claim.DetermineInitialReviewStatus(dto.BilledAmount),
            SubmittedAt = DateTime.UtcNow
        };

        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetClaim), new { id = claim.Id }, claim);
    }

    // PATCH: api/claims/5/resolve — a reviewer approves or denies
    [HttpPatch("{id}/resolve")]
    public async Task<IActionResult> ResolveClaim(int id, [FromBody] ResolveClaimRequest request)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null) return NotFound();

        claim.Status = request.Approve ? ClaimStatus.Approved : ClaimStatus.Denied;
        claim.ApprovedAmount = request.Approve ? request.ApprovedAmount : null;
        claim.ReviewNotes = request.Notes;
        claim.ResolvedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class ResolveClaimRequest
{
    public bool Approve { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? Notes { get; set; }
}
