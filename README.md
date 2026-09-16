# Healthcare Claims API

A .NET 8 Web API modeling a simplified healthcare payer workflow — patients, appointments, and insurance claims — with an automated audit-flagging rule inspired by real payer/utilization-review processes.

**Live demo:** http://abdul05-001-site1.ftempurl.com/swagger

## What it does

The API models a realistic chain: a **Patient** books an **Appointment**, and that appointment generates an insurance **Claim**. When a claim is submitted, a domain rule automatically routes it:

- Claims under $5,000 → `UnderReview`
- Claims of $5,000 or more → `FlaggedForAudit` (mirrors real fraud/waste/abuse review triggers)

A reviewer can then resolve any claim as `Approved` or `Denied` via a dedicated endpoint.

## Tech stack

- **.NET 8** / **ASP.NET Core** — both MVC-style controllers and a Minimal API endpoint (`/health`)
- **Entity Framework Core** with **SQL Server** — code-first models, migrations, fluent relationship configuration
- **Swagger / OpenAPI** for interactive API documentation
- Deployed on live SQL Server + IIS hosting via FTP

## Project structure

```
HealthcareClaimsApi/
├── Controllers/     API endpoints (Patients, Appointments, Claims) — MVC-style [ApiController]s
├── Models/           EF Core entities: Patient, Appointment, Claim (with the audit-threshold rule)
├── Dtos/              Request DTOs — keep the API input clean, separate from the database entities
├── Data/               AppDbContext — EF Core setup and entity relationship configuration
├── Migrations/     EF Core-generated database schema history
├── Properties/       launchSettings.json — local run configuration
├── Program.cs        App startup: DI, EF Core registration, Swagger, routing
└── appsettings.json  Configuration (connection string — not committed; see .gitignore)
```

## Architecture

```
Patient (1) ──< Appointment (1) ──1:1── Claim
```

- `Patient` has many `Appointment`s
- `Appointment` has one `Claim`
- Request/response DTOs (`Dtos/RequestDtos.cs`) keep the API surface clean and avoid circular-reference serialization issues between the entities

## API endpoints

| Resource | Endpoints |
|---|---|
| Patients | `GET /api/patients`, `GET /api/patients/{id}`, `POST /api/patients`, `PUT /api/patients/{id}`, `DELETE /api/patients/{id}` |
| Appointments | `GET /api/appointments`, `GET /api/appointments/{id}`, `POST /api/appointments`, `PATCH /api/appointments/{id}/status` |
| Claims | `GET /api/claims` (filterable by `?status=`), `GET /api/claims/{id}`, `POST /api/claims`, `PATCH /api/claims/{id}/resolve` |
| Health | `GET /health` |

## Try the live demo

1. Open the [Swagger UI](http://abdul05-001-site1.ftempurl.com/swagger)
2. `POST /api/patients` — create a patient
3. `POST /api/appointments` — book them an appointment, using the patient's returned `id`
4. `POST /api/claims` — submit a claim for that appointment with `billedAmount: 6000` — it comes back `FlaggedForAudit` instead of `UnderReview`, since it crossed the $5,000 threshold in code
5. `PATCH /api/claims/{id}/resolve` — approve or deny it

## Running it locally

```bash
git clone <this-repo-url>
cd HealthcareClaimsApi
dotnet restore
```

Update the `DefaultConnection` string in `appsettings.json` to point at your own SQL Server instance (LocalDB, Docker, or Azure SQL all work), then:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Open `http://localhost:5000/swagger`.

## Notes on the deployment

This was originally planned for Azure App Service + Azure SQL Database, but Azure account signup requires a credit card for identity verification, which wasn't available. Rather than block on that, the project was deployed to a SQL Server + IIS host instead (SmarterASP.NET), which meant working through:

- Connection string differences between database providers
- EF Core migration regeneration when switching providers
- IIS-specific deployment layout (`web.config`, correct site-root folder placement)
- Fixing a Swagger example generator issue caused by circular EF Core navigation properties, by introducing request DTOs
- HTTPS redirect behavior on a host without a valid certificate for the assigned subdomain

## License

MIT
