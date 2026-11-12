# LabEquipmentApi — Laboratory Equipment Inventory

**Server-Side Applications, bonus practice exercise (items 1–5 + optional item 6).**

ASP.NET Core 10 Web API using controllers, PostgreSQL, EF Core Code First (with a checked-in initial migration), Swagger UI, Mapster and FluentValidation.

## Requirements

- Windows, Linux or macOS with **.NET 10 SDK**
- PostgreSQL running locally on port **5432** (or adjust the connection string)
- EF Core CLI (`dotnet ef --version` should show version 10.x)

## Database configuration — do not commit passwords

`appsettings.json` contains a sample connection string without a password, pointing to database `lab_equipment` and role `postgres`.

On Windows PowerShell, set a connection string with **your own PostgreSQL password** for the current terminal session:

```powershell
$env:ConnectionStrings__LabDbConnection = "Host=localhost;Port=5432;Database=lab_equipment;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
```

Alternatively, set the value with .NET user secrets from this project folder:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:LabDbConnection" "Host=localhost;Port=5432;Database=lab_equipment;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
```

Create an empty PostgreSQL database called `lab_equipment` first if your PostgreSQL role cannot create databases. `dotnet ef database update` can usually create the database when the role has permissions.

## Build and apply the included migration

```powershell
dotnet restore
dotnet build LabEquipmentApi.slnx
dotnet tool install --global dotnet-ef --version 10.0.11   # only if missing
dotnet ef database update
dotnet run --launch-profile https
```

The first migration has **already been included** in `Migrations/`, so there is no need to run `dotnet ef migrations add InitialCreate` again.

Open the HTTPS address reported by `dotnet run` and navigate to `/swagger` (typically `https://localhost:7222/swagger`).

## Required endpoints

| Method | Path | Behavior |
| --- | --- | --- |
| GET | `/api/equipment` | All equipment; three seeded items initially |
| GET | `/api/equipment/{id}` | Single item; `1` = Projector, `999` = 404 |
| POST | `/api/equipment` | Request DTO `{ "name":"Keyboard", "room":"Lab 10" }`; validates fields; creates available equipment; 201 + Location header |
| PUT | `/api/equipment/{id}` | Full CRUD update (scaffold style) |
| DELETE | `/api/equipment/{id}` | Delete a row |
| GET | `/api/equipment/available` | Mapster DTO projection: Monitor, then Projector; no Laptop |
| GET | `/api/equipment/room-summary` | **Optional:** Lab 10 = 2 total / 1 available; Lab 11 = 1 total / 1 available |

Validation rules on POST: Name is required and at most 100 characters; Room is required and at most 20 characters; `Id` and `IsAvailable` cannot be supplied through the create request DTO.

## Quick API checks

Start the application with the Development environment, then in another PowerShell window run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-LabEquipmentApi.ps1 -BaseUrl https://localhost:7222 -SkipCertificateCheck
```

Use a different `-BaseUrl` if the port shown by `dotnet run` differs.

## Implementation history

Meaningful commits separately implement each assignment section. Inspect them with `git log --oneline`.

## Notes

- All package versions follow `LabEquipment_EN_final.pdf` (EF Core 10.0.11; Npgsql provider 10.0.3).
- No SQL Server provider is used.
- The DbContext, generated-style CRUD controller, response DTOs, request DTO, validator, Swagger UI, and migration are checked into source control.
- This repository is a teaching exercise. For production, use secret managers and avoid exposing equipment mutation routes without authentication.
