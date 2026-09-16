namespace HealthcareClaimsApi.Models;

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? InsuranceMemberId { get; set; }

    // Navigation property: one patient can have many appointments
    public List<Appointment> Appointments { get; set; } = new();
}
