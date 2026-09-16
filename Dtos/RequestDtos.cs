namespace HealthcareClaimsApi.Dtos;

public record CreatePatientDto(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string? InsuranceMemberId
);

public record CreateAppointmentDto(
    int PatientId,
    DateTime ScheduledAt,
    string ProviderName,
    string ReasonForVisit
);

public record CreateClaimDto(
    int AppointmentId,
    decimal BilledAmount,
    string ProcedureCode
);
