namespace HealthcareClaimsApi.Models;

public enum ClaimStatus
{
    Submitted,
    UnderReview,
    Approved,
    Denied,
    FlaggedForAudit
}

public class Claim
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public decimal BilledAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string ProcedureCode { get; set; } = string.Empty; // e.g. CPT code
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    public string? ReviewNotes { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Simple domain rule: claims above a threshold get auto-flagged for audit
    // rather than auto-approved. This is the kind of "fraud/waste/abuse" logic
    // the JD calls out.
    public static ClaimStatus DetermineInitialReviewStatus(decimal billedAmount)
    {
        const decimal AuditThreshold = 5000m;
        return billedAmount >= AuditThreshold
            ? ClaimStatus.FlaggedForAudit
            : ClaimStatus.UnderReview;
    }
}
