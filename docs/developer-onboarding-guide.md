# DemoProject - Developer Onboarding Guide

Welcome to DemoProject. This guide introduces the architecture, coding standards, folder structure, and step-by-step workflows for evolving the project (entities, tables, services, requests/responses, mappings, controllers, DI, and EF Core migrations).

- Tech stack: .NET 8, ASP.NET Core Web API, Entity Framework Core (SQL Server), Mapster for mappings, Swagger for API docs.
- Solution layout: Web API app in applications/, domain/services/infrastructure libraries in libraries/.

## Solution Structure

```
DemoProject.sln
applications/
  DemoProject.WebApi/
    Controllers/
      DemosController.cs
    HttpFiles/
      DemoController.http
    Middleware/
      GlobalExceptionHandlerMiddleware.cs
    Program.cs
    appsettings.json
    appsettings.Development.json
libraries/
  DemoProject.Domain/
    Tables/
      DemoTable.cs
      OptionTable.cs
    Exceptions/
      NotFoundException.cs
      UnauthorizedException.cs
      UnexpectedErrorException.cs
      ValidationException.cs
    Constants/
    Enums/
  DemoProject.Infrastructure/
    Data/
      ApplicationDbContext.cs
      DataRepository.cs
      MigrationCommands.txt
      Migrations/
        <timestamp>_InitialMigration.cs
        <timestamp>_InitialMigration.Designer.cs
        ApplicationDbContextModelSnapshot.cs
      TableConfigurations/
        DemoTableConfiguration.cs
    InfrastructureCollectionExtensions.cs
    DemoProject.Infrastructure.csproj
  DemoProject.Services/
    _Shared/
      InfrastructureInterfaces/
        IDataRepository.cs
      Requests/
        SearchRequest.cs
      Responses/
        PageResponse.cs
        Demo.cs
        Option.cs
    Demo/
      IDemoService.cs
      DemoService.cs
      DemoMappingConfiguration.cs
      Requests/
        SaveDemoRequest.cs
        CreateDemoRequest.cs
        UpdateDemoRequest.cs
        GetDemoByIdRequest.cs
        GetAllDemosRequest.cs
        SearchDemosRequest.cs
        DeleteDemoRequest.cs
      Responses/
        GetDemoByIdResponse.cs
        GetAllDemosResponse.cs
        SearchDemosResponse.cs
        CreateDemoResponse.cs
        UpdateDemoResponse.cs
        DeleteDemoResponse.cs
```

### Folder placement guidelines

- /applications: All deployable, runnable apps (e.g., Web APIs, UI clients, background workers/processes, services, gateways).
- /libraries: All class libraries (domain, services, infrastructure, shared packages).
- /tests: All automated tests (unit, integration, end-to-end). Create this folder if/when tests are added.

### Libraries overview

- Domain (`libraries/DemoProject.Domain`)

  - Purpose: Core domain model (entities) and domain-specific enums, constants, exceptions. Pure C#; no EF attributes or infrastructure/UI dependencies.
  - Contents: `Entities/`, `Exceptions/`, `Constants/`, `Enums/`.

- Services (`libraries/DemoProject.Services`)

  - Purpose: Application layer. Business use-cases, service interfaces/implementations, feature-specific Requests/Responses, and Mapster mappings.
  - Contents: Feature folders (e.g., `Demo/`), `_Shared/Requests`, `_Shared/Responses`, `_Shared/InfrastructureInterfaces` (e.g., `IDataRepository<T>`), `ServiceCollectionExtensions` for DI.
  - Notes: References Domain; uses Mapster; throws domain exceptions; no EF Core or DbContext code here.

- Infrastructure (`libraries/DemoProject.Infrastructure`)
  - Purpose: External infrastructure concerns (e.g., DataAccess, external services, external tools and libraries).
  - Contents: `Data/ApplicationDbContext`, `Data/TableConfigurations` (`IEntityTypeConfiguration<>`), `Data/Migrations`, `Data/DataRepository<T>`, `InfrastructureCollectionExtensions` for DI.
  - Notes: Implements `IDataRepository<T>`, configures EF Core (SQL Server), owns migrations; references Domain (entities) and Services (interfaces only).

## Architecture Overview

- Domain (libraries/DemoProject.Domain): Pure domain entities and domain exceptions only. Pure C#; No EF or UI dependencies.
- Infrastructure (libraries/DemoProject.Infrastructure): EF Core DbContext, table configurations, generic repository, and infrastructure DI.
- Services (libraries/DemoProject.Services): Application layer. Business logic services, DTOs (Requests/Responses), mapping configuration, and service DI.
- WebApi (applications/DemoProject.WebApi): HTTP endpoints, middleware, Program/DI composition, Swagger.

## Coding Standards and Conventions

- Projects target net8.0; nullable disabled in this repo.
- Mapping via Mapster. Configure mappings once in DemoMappingConfiguration and call DemoProject.Services.AddServiceDependencies().
- Requests/Responses per feature live under libraries/DemoProject.Services/<Feature>/Requests and .../Responses.
- Repository pattern exposed via IDataRepository<T> in Services.\_Shared.InfrastructureInterfaces and implemented by Infrastructure.DataRepository<T>.
- EF Core configuration by IEntityTypeConfiguration<T> classes under Infrastructure/Data/TableConfigurations. DbContext applies all configurations via ApplyConfigurationsFromAssembly.
- Exceptions bubble to the API and are translated to RFC7807 problem+json by GlobalExceptionHandlerMiddleware.
- Controllers should be thin, delegate to services, and return typed response models.

## Running Locally

- Configure connection string DefaultConnection in applications/DemoProject.WebApi/appsettings.Development.json.
- Build and run from the Web API project. Swagger UI is enabled in Development.
- Database (Docker required): Ensure Docker Desktop is installed and running. From the repo root, start the local SQL Server container defined in `docker-compose.yml`:
  - `docker compose up -d`
  - The compose file maps host port 1434 to container port 1433 and sets SA password. Example connection string for DefaultConnection:
    - `Server=localhost,1434;Database=DemoDb;User Id=sa;Password=LocalServer1!;TrustServerCertificate=True;`

## Dependency Injection

- In Program.cs:
  - AddDbContext<ApplicationDbContext>(UseSqlServer(..., b.MigrationsAssembly("DemoProject.Infrastructure")))
  - services.AddServiceDependencies() registers application services, and Mapster configs.
  - services.AddInfrastructureDependencies() registers IDataRepository<T>, and other external Infrastructure concerns.

## EF Core: Tables, Entities, and Migrations

This project uses EF Core Code-First with SQL Server.

1. Add or Modify a Table (via configuration)

- Create or update an IEntityTypeConfiguration<T> under Infrastructure/Data/TableConfigurations.
- Example: `DemoTableConfiguration` sets schema/table ("Demo","Demo"), key, identities, and constraints.

2. Add or Modify an Entity

- Add/modify a POCO in libraries/DemoProject.Domain/Entities. Example `DemoEntity` or `OptionEntity`.
- Do not add EF attributes here; must use fluent configuration in TableConfigurations.

3. Create a Migration

- From repo root, change directory to Infrastructure project.
  - Windows PowerShell:
    - cd .\libraries\DemoProject.Infrastructure\
- Use commands listed in MigrationCommands.txt (must be run from Infrastructure dir):
  - dotnet ef migrations add <MigrationName> --context ApplicationDbContext --output-dir Data/Migrations --startup-project ../../applications/DemoProject.WebApi
  - dotnet ef database update --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi

4. Validate

- Check generated files in Infrastructure/Data/Migrations and run the application.

## Generic Repository Usage

- Interface: Services/\_Shared/InfrastructureInterfaces/IDataRepository<T>
- Implementation: Infrastructure/Data/DataRepository.cs
- Supports: GetById, GetAll, Get(predicate), Add/Range, Update, Remove/Range, Search(paged + orderBy), and explicit transactions (Begin/Commit/Rollback).
- In services, inject IDataRepository<DemoEntity> (or other entity) and use methods. Search supports optional filter and dynamic ordering.

## Adding a New Feature (Service) End-to-End

Assume a new feature "Products" with an entity `ProductEntity`.

1. Domain Entity

- Add libraries/DemoProject.Domain/Entities/ProductEntity.cs with scalar properties and keys.

2. Table Configuration

- Add libraries/DemoProject.Infrastructure/Data/TableConfigurations/ProductTableConfiguration.cs implementing IEntityTypeConfiguration<ProductEntity>.
- Configure schema/table name, keys, indexes, constraints.

3. Migration

- Run EF migration commands (see above) to generate and apply DB changes.

4. DTOs (Requests/Responses)

- Under libraries/DemoProject.Services/Product/Requests:
  - SaveProductRequest (validation attributes)
  - CreateProductRequest : SaveProductRequest
  - UpdateProductRequest : SaveProductRequest + ProductId
  - GetProductByIdRequest, GetAllProductsRequest, SearchProductsRequest : SearchRequest + Query
  - DeleteProductRequest
- Under .../Responses:
  - GetProductByIdResponse, GetAllProductsResponse, SearchProductsResponse, CreateProductResponse, UpdateProductResponse, DeleteProductResponse
- Add shared response DTO under Services/\_Shared/Responses if it’s reused across features.

5. Mapping

- Add ProductMappingConfiguration.ConfigureMappings using Mapster TypeAdapterConfig:
  - Entity -> Shared DTO
  - Create/Update requests -> Entity
- Call it inside Services.AddServiceDependencies() alongside DemoMappingConfiguration.

6. Service Interface and Implementation

- Add IProductService with methods for each operation.
- Implement ProductService using IDataRepository<ProductEntity>.
- Follow DemoService patterns: validate existence, throw NotFoundException, use repository, adapt entities/DTOs, support Search with orderBy string mapping function.

7. Register in DI

- In Services.AddServiceDependencies(): services.AddScoped<IProductService, ProductService>(); and invoke ProductMappingConfiguration.

8. Controller

- Add ProductsController mirroring DemosController:
  - GET /api/products/{id}
  - GET /api/products
  - POST /api/products/search
  - POST /api/products
  - PUT /api/products/{id}
  - DELETE /api/products/{id}
- Controllers take/return the typed request/response models and delegate to the service.

## Request/Response Standards

> Every service operation in the service layer must define explicit Request and Response classes. Each method in a service interface accepts a Request and returns a Response. Do not pass primitives or domain entities across the service boundary.

- Placement: Feature-specific types live under `libraries/DemoProject.Services/<Feature>/Requests` and `.../<Feature>/Responses`.
- Reuse: Prefer composition or inheritance using shared bases in `_Shared/Requests` and `_Shared/Responses`.
  - Composition examples: include `SearchRequest` for paging/sorting; return lists/items using shared DTOs (e.g., `_Shared.Responses.Demo`).
  - Inheritance examples: `CreateXRequest : SaveXRequest`; `UpdateXRequest : SaveXRequest` plus the key (e.g., `XId`).
- Naming: `<Operation>Name>Request` and `<Operation>Name>Response` (e.g., `GetDemoByIdRequest`, `GetDemoByIdResponse`).
- Validation: Put data annotations (e.g., `[Required]`, `[StringLength]`) on Request types, not on domain entities.

## Mapster Mapping Standards

- Define mappings in <Feature>MappingConfiguration.
- Keep mappings 1:1 with DTOs/entities.
- In AddServiceDependencies(), call all ConfigureMappings() once at startup.
- In services, use Adapt<T>() for conversions.

## Transactions

- Repository auto-saves if there is no ambient transaction.
- For multi-step operations requiring atomicity:
  - await BeginTransaction(); perform multiple Add/Update/Remove; await CommitTransaction();
  - On errors, await RollbackTransaction().

## Error Handling

- Throw domain exceptions from Services:
  - NotFoundException for missing entities.
  - ValidationException for invalid requests.
  - UnauthorizedException where applicable.
- API converts exceptions to ProblemDetails via GlobalExceptionHandlerMiddleware.

## Adding DI for New Components

- Services: register new service interfaces in Services.AddServiceDependencies().
- Infrastructure: register new repositories or infrastructure services in Infrastructure.AddInfrastructureDependencies().
- Program.cs wires both extension methods and the DbContext.

## Swagger and HTTP Files

- Swagger is enabled in Development.
- Sample HTTP requests are in applications/DemoProject.WebApi/HttpFiles/DemoController.http.

## Checklist for Adding a New Resource

- Domain entity added.
- Table configuration created and migration generated/applied.
- Service requests/responses/mapping created.
- Service interface and implementation added; added to DI.
- Controller endpoints added.
- Manual tests via Swagger or .http file; add unit tests as needed.

## EF Core Commands Quick Reference (Windows PowerShell)

Run from libraries/DemoProject.Infrastructure directory:

- dotnet ef migrations add AddProductTable --context ApplicationDbContext --output-dir Data/Migrations --startup-project ../../applications/DemoProject.WebApi
- dotnet ef database update --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi
