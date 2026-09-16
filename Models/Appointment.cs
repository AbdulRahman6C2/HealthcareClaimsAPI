namespace HealthcareClaimsApi.Models;

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled,
    NoShow
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public DateTime ScheduledAt { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ReasonForVisit { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    // Navigation property: one appointment can generate one claim
    public Claim? Claim { get; set; }
}
