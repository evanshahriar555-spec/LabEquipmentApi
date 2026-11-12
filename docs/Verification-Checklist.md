# LabEquipmentApi — Assignment and course-lab alignment

This checklist maps the **LabEquipment_EN_final.pdf** assignment to its referenced **lab-01.pdf** through **lab-04.pdf** examples. This is a source-code review, **not** a claim that a .NET build or database test has been executed in this environment. The workflow under `.github/workflows/verify.yml` runs those checks after the project is pushed to GitHub.

| Assignment requirement | Implemented location | Earlier lab pattern |
| --- | --- | --- |
| 1. ASP.NET Core Web API, .NET 10, controllers, OpenAPI | `LabEquipmentApi.csproj`, `Program.cs`, `LabEquipmentApi.slnx` | Lab 1 project and entry point; Lab 3 controllers |
| 2. PostgreSQL with EF Core Code First | `LabEquipmentApi.csproj`, `Data/LabDbContext.cs`, `appsettings.json` | Lab 1 EF Core, DbContext, `UseNpgsql` |
| 2. Required `Equipment` entity and three seed records | `Domain/Equipment.cs`, `Data/LabDbContext.cs` | Lab 1 entities and `HasData` |
| 2. Initial migration | `Migrations/20261112000000_InitialCreate.cs`, associated model files | Lab 1 migration workflow |
| 3. Scaffold-style REST CRUD routes, Swagger UI | `Controllers/EquipmentController.cs`, `Program.cs` | Lab 3 generated CRUD, OpenAPI/Swagger configuration |
| 4. Available equipment, sorted, projected into DTO | `Contracts/Equipment/EquipmentListItemResponse.cs`, `GetAvailableEquipment()` | Lab 4 `Where`, `ProjectToType<T>`, async queries |
| 5. Create request DTO and FluentValidation | `CreateEquipmentRequest.cs`, `CreateEquipmentRequestValidator.cs`, `PostEquipment()` and `Program.cs` | Lab 4 DTO/validation approach |
| 6. Optional room-summary | `RoomSummaryResponse.cs`, `GetRoomSummary()` | Additional optional exercise |
| Several sequential Git commits | Git history in `LabEquipmentApi-Complete-With-Git-History.zip` | Assignment submission instructions |

## Required behavior to verify when running the app

1. Apply `dotnet ef database update` against **PostgreSQL**, with the connection string supplied locally. Confirm three rows: Projector (Lab 10, available), Laptop (Lab 10, unavailable), Monitor (Lab 11, available).
2. `GET /api/equipment` returns the three rows. `GET /api/equipment/1` returns Projector. `/api/equipment/999` returns 404.
3. `GET /api/equipment/available` returns **Monitor** then **Projector** and excludes Laptop. It returns DTO properties `id`, `name`, `room`, not availability.
4. `POST /api/equipment` with `{ "name":"Keyboard", "room":"Lab 10" }` returns 201 with a `Location` header pointing to `/api/equipment/{id}`. Its `isAvailable` value is true, and the item appears in `/available`.
5. `POST /api/equipment` with an empty name or room returns 400 with validation information.
6. Optional: `/api/equipment/room-summary` initially returns Lab 10 = 2 total / 1 available and Lab 11 = 1 total / 1 available.
7. Check that GitHub Actions **Build and test LabEquipmentApi** passes and the repository is **public** under `evanshahriar555-spec/LabEquipmentApi`, then submit that URL in Teams.

## Notes

- The original assignment asks for a *scaffolded* controller. The checked-in controller follows the scaffolded CRUD structure and adds the required custom actions; if your lecturer requires evidence that the generator tool itself was run, the checked-in code alone does not prove that action.
- The initial migration is included as source, so do **not** generate a duplicate `InitialCreate` migration before applying it.
- `appsettings.json` deliberately omits database passwords. Supply your password via an environment variable or .NET user secrets as described in `README.md`.
- The repository has not been created or uploaded by this package; use `Publish-LabEquipmentApi.cmd` to build and push its commit history from Windows.
