# DemoProject - Developer Onboarding Guide

Welcome to DemoProject. This guide introduces the architecture, coding standards, folder structure, and step-by-step workflows for evolving the project (entities, tables, services, requests/responses, mappings, controllers, DI, and EF Core migrations).

- Tech stack: .NET 8, ASP.NET Core Web API, Entity Framework Core (SQL Server), Mapster for mappings, Swagger for API docs.
- Solution layout: Web API app in applications/, data/services/infrastructure libraries in libraries/.

## Solution Structure

```
DemoProject.sln
src/
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
    DemoProject.Data/
      DbContext/
        ApplicationDbContext.cs
      Models/
        Demo.cs
      Repository/
        Interfaces/
          Base/
            IRepository.cs
            IRepositoryWithSearch.cs
            IUnitOfWork.cs
            Models/
              SearchRequest.cs
              SearchResponse.cs
          IDemoRepository.cs
        Implementation/
          Base/
            BaseRepository.cs
            BaseRepositoryWithSearch.cs
            UnitOfWork.cs
          DemoRepository.cs
      TableConfigurations/
        DemoTableConfiguration.cs
      Migrations/
        <timestamp>_InitialMigration.cs
        <timestamp>_InitialMigration.Designer.cs
        ApplicationDbContextModelSnapshot.cs
      MigrationCommands.txt
      DependencyInjectionDependencies.cs
    DemoProject.Infrastructure/
      Modules/
        EmailService/
          IEmailService.cs
      DependencyInjectionExtensions.cs
    DemoProject.Services/
      Modules/
        Demo/
          IDemoService.cs
          DemoService.cs
          DemoMappingConfiguration.cs
          Requests/
            CreateDemoRequest.cs
            UpdateDemoRequest.cs
            GetDemoByIdRequest.cs
            GetAllDemosRequest.cs
            SearchDemosRequest.cs
            DeleteDemoRequest.cs
          Responses/
            Demo.cs
            GetDemoByIdResponse.cs
            GetAllDemosResponse.cs
            SearchDemosResponse.cs
            CreateDemoResponse.cs
            UpdateDemoResponse.cs
            DeleteDemoResponse.cs
      Shared/
        Exceptions/
          NotFoundException.cs
          UnauthorizedException.cs
          UnexpectedErrorException.cs
          ValidationException.cs
        Models/
          SearchRequest.cs
          SearchResponse.cs
      DependencyInjectionExtensions.cs
tests/
```

### Folder placement guidelines

- **/src/applications**: All deployable, runnable apps (e.g., Web APIs, UI clients, background workers/processes, services, gateways).
- **/src/libraries**: All class libraries organized as common/shared modules (Data, Infrastructure) and service modules (Services).
- **/tests**: All automated tests (unit, integration, end-to-end). Create this folder if/when tests are added.

### Modular Architecture

This solution follows a **modular workspace architecture** with distinct separation between common/shared modules and service modules:

#### Common/Shared Modules (Can be used by multiple services)

- **Data** (`libraries/DemoProject.Data`)

  - Purpose: Data access layer. Contains EF Core DbContext, entity models, repositories, table configurations, and migrations.
  - Contents: `DbContext/`, `Models/`, `Repository/`, `TableConfigurations/`, `Migrations/`, `DependencyInjectionDependencies.cs`
  - Dependencies: EF Core packages, no other project references
  - Notes: Repository pattern with base interfaces (`IRepository<TEntity, TKey>`, `IRepositoryWithSearch<TEntity, TKey, TSearchFilters>`, `IUnitOfWork`). All repositories can be used by multiple services.

- **Infrastructure** (`libraries/DemoProject.Infrastructure`)
  - Purpose: External infrastructure concerns (e.g., email services, Azure Storage, external APIs, third-party tools).
  - Contents: `Modules/` (e.g., `EmailService/`), `DependencyInjectionExtensions.cs`
  - Dependencies: Microsoft.Extensions.DependencyInjection.Abstractions, external service SDKs
  - Notes: Contains interfaces and implementations for infrastructure services. Can be used by multiple services.

#### Service Modules (Business logic modules)

- **Services** (`libraries/DemoProject.Services`)
  - Purpose: Application/business layer. Service interfaces, implementations, feature-specific Requests/Responses, mappings, and business exceptions.
  - Contents: `Modules/<FeatureName>/` (e.g., `Modules/Demo/`), `Shared/` (Exceptions, Models), `DependencyInjectionExtensions.cs`
  - Dependencies: References `DemoProject.Data` and `DemoProject.Infrastructure`; uses Mapster
  - Notes: Each module represents a bounded context or business feature. Services throw business exceptions defined in `Shared/Exceptions/`.

## Architecture Overview

This solution implements a **clean, modular architecture** with strict separation of concerns:

### Layer Dependencies (top to bottom)

1. **WebApi** (applications/DemoProject.WebApi)

   - HTTP endpoints, controllers, middleware, Program/DI composition, Swagger
   - **Can reference**: Services, Infrastructure, Data
   - **Responsibility**: HTTP layer only; thin controllers that delegate to services

2. **Services** (libraries/DemoProject.Services)

   - Application/business layer with service modules organized by business features
   - **Can reference**: Data (repositories), Infrastructure (external services)
   - **Responsibility**: Business logic, validation, orchestration, mapping DTOs

3. **Infrastructure** (libraries/DemoProject.Infrastructure) - Common Module

   - External infrastructure concerns (email, storage, external APIs, third-party services)
   - **Can reference**: No other project references (standalone)
   - **Responsibility**: Infrastructure service interfaces and implementations
   - **Shared by**: Multiple service modules

4. **Data** (libraries/DemoProject.Data) - Common Module
   - Data access layer with EF Core, repositories, entities, migrations
   - **Can reference**: No other project references (standalone)
   - **Responsibility**: Database operations, entity configurations, migrations
   - **Shared by**: Multiple service modules via repository interfaces

### Critical Architectural Rules

1. **One Controller → One Service (1:1)**

   - Each controller uses **only one** service
   - Each service is used by **only one** controller
   - Example: `DemosController` → `IDemoService` only

2. **Services Cannot Reference Other Services**

   - Service modules are isolated and cannot depend on each other
   - Services communicate only through common modules (Data repositories, Infrastructure services)
   - Example: `DemoService` cannot reference `ProductService`

3. **Repositories Are Shared**

   - Multiple services can use the same repository
   - Repositories are defined in the Data common module
   - Example: Both `DemoService` and `ProductService` can use `IDemoRepository`

4. **Service Modules Use Only Common Modules**
   - Service modules can only reference: `DemoProject.Data` and `DemoProject.Infrastructure`
   - Service modules **cannot** reference other service modules
   - This ensures loose coupling and independent evolution of features

## Coding Standards and Conventions

- Projects target net8.0; nullable disabled in this repo.
- Mapping via Mapster. Configure mappings once in `<Feature>MappingConfiguration` and call `DemoProject.Services.AddApplicationServices()`.
- Requests/Responses per feature live under `libraries/DemoProject.Services/Modules/<Feature>/Requests` and `.../Responses`.
- Shared response models live in `libraries/DemoProject.Services/Shared/Models/`.
- Repository pattern:
  - Base interfaces defined in `DemoProject.Data/Repository/Interfaces/Base/` (`IRepository<TEntity, TKey>`, `IRepositoryWithSearch<TEntity, TKey, TSearchFilters>`, `IUnitOfWork`)
  - Feature-specific repositories in `DemoProject.Data/Repository/Interfaces/` (e.g., `IDemoRepository`)
  - Implementations in `DemoProject.Data/Repository/Implementation/`
  - **Multiple services can use the same repository**
- EF Core configuration by `IEntityTypeConfiguration<T>` classes under `Data/TableConfigurations`. DbContext applies all configurations via `ApplyConfigurationsFromAssembly`.
- Entity models (POCOs) defined in `DemoProject.Data/Models/` (e.g., `Demo.cs`)
- Business exceptions defined in `DemoProject.Services/Shared/Exceptions/`
- Exceptions bubble to the API and are translated to RFC7807 problem+json by `GlobalExceptionHandlerMiddleware`.
- Controllers should be thin, delegate to services, and return typed response models.
- **One controller uses only one service; one service is used by only one controller.**
- **Services cannot reference or use other services; only repositories can be shared across services.**

## Running Locally

- Configure connection string DefaultConnection in applications/DemoProject.WebApi/appsettings.Development.json.
- Build and run from the Web API project. Swagger UI is enabled in Development.
- Database (Docker required): Ensure Docker Desktop is installed and running. From the repo root, start the local SQL Server container defined in `docker-compose.yml`:
  - `docker compose up -d`
  - The compose file maps host port 1434 to container port 1433 and sets SA password. Example connection string for DefaultConnection:
    - `Server=localhost,1434;Database=DemoDb;User Id=sa;Password=LocalServer1!;TrustServerCertificate=True;`

## Dependency Injection

In `Program.cs`, the DI container is configured with the following extension methods:

```csharp
// EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("DemoProject.Data")));

// Infrastructure services (email, storage, external APIs)
builder.Services.AddInfrastructureServices();

// Data repositories (IUnitOfWork, IDemoRepository, etc.)
builder.Services.AddRepositoryServices();

// Application services (IDemoService, etc.) and Mapster configurations
builder.Services.AddApplicationServices();
```

### DI Extension Methods by Module

- **DemoProject.Data.AddRepositoryServices()**

  - Registers `IUnitOfWork`, `IDemoRepository`, and other repository interfaces
  - Defined in `DemoProject.Data/DependencyInjectionDependencies.cs`

- **DemoProject.Infrastructure.AddInfrastructureServices()**

  - Registers infrastructure services (e.g., `IEmailService`)
  - Defined in `DemoProject.Infrastructure/DependencyInjectionExtensions.cs`

- **DemoProject.Services.AddApplicationServices()**
  - Registers all service interfaces (e.g., `IDemoService`)
  - Configures Mapster mappings for all features
  - Defined in `DemoProject.Services/DependencyInjectionExtensions.cs`

## EF Core: Models, Tables, and Migrations

This project uses EF Core Code-First with SQL Server. All database-related code lives in the **DemoProject.Data** common module.

### 1. Add or Modify an Entity Model

- Add/modify a POCO in `libraries/DemoProject.Data/Models/`.
- Example: `Demo.cs` with properties like `DemoId`, `Name`, etc.
- Do not add EF attributes here; use fluent configuration in TableConfigurations.

### 2. Add or Modify a Table Configuration

- Create or update an `IEntityTypeConfiguration<T>` under `Data/TableConfigurations`.
- Example: `DemoTableConfiguration` sets schema/table name, primary key, indexes, and constraints.

```csharp
public class DemoTableConfiguration : IEntityTypeConfiguration<Demo>
{
    public void Configure(EntityTypeBuilder<Demo> builder)
    {
        builder.ToTable("Demos", "dbo");
        builder.HasKey(e => e.DemoId);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
    }
}
```

### 3. Update DbContext

- Add a `DbSet<T>` property in `Data/DbContext/ApplicationDbContext.cs` if needed.
- The `OnModelCreating` method automatically applies all configurations via `ApplyConfigurationsFromAssembly`.

### 4. Create a Migration

- From repo root, navigate to the Data project directory:
  ```powershell
  cd .\src\libraries\DemoProject.Data\
  ```
- Use commands listed in `MigrationCommands.txt` (must be run from Data directory):
  ```powershell
  dotnet ef migrations add <MigrationName> --context ApplicationDbContext --output-dir Migrations --startup-project ../../applications/DemoProject.WebApi
  ```
- Apply the migration:
  ```powershell
  dotnet ef database update --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi
  ```

### 5. Validate

- Check generated files in `Data/Migrations/`
- Run the application and verify the database schema

## Repository Pattern Usage

The solution uses a flexible repository pattern with base interfaces and feature-specific repositories.

### Base Interfaces (in DemoProject.Data)

- **IRepository<TEntity, TKey>**

  - Basic CRUD operations: `GetById`, `GetAll`, `Create`, `Update`, `Delete`
  - Located in `Data/Repository/Interfaces/Base/`

- **IRepositoryWithSearch<TEntity, TKey, TSearchFilters> : IRepository<TEntity, TKey>**

  - Extends `IRepository` with search capabilities
  - Includes `Search(SearchRequest<TSearchFilters>)` method
  - Supports pagination, filtering, and sorting

- **IUnitOfWork**
  - Transaction management: `BeginTransaction()`, `CommitTransaction()`, `RollbackTransaction()`
  - Use for multi-step operations requiring atomicity

### Feature-Specific Repositories

Define feature-specific repositories by extending base interfaces:

```csharp
// In Data/Repository/Interfaces/IDemoRepository.cs
public interface IDemoRepository : IRepositoryWithSearch<Demo, int, DemoSearchFilters>
{
    // Add custom methods specific to Demo entity if needed
}

public class DemoSearchFilters
{
    public string Name { get; set; }
}
```

### Implementation

- Base implementations in `Data/Repository/Implementation/Base/`
- Feature implementations in `Data/Repository/Implementation/`
- Implementations use `ApplicationDbContext` for database operations

### Usage in Services

```csharp
public class DemoService : IDemoService
{
    private readonly IDemoRepository _demoRepository;

    public DemoService(IDemoRepository demoRepository)
    {
        _demoRepository = demoRepository;
    }

    public async Task<GetDemoByIdResponse> GetDemoById(GetDemoByIdRequest request)
    {
        var entity = await _demoRepository.GetById(request.Id);
        // ... map and return response
    }
}
```

### Key Points

- **Repositories are shared**: Multiple services can inject and use the same repository
- **Services are isolated**: Services cannot reference other services, only repositories
- Repositories are registered in `DemoProject.Data.AddRepositoryServices()`

## Adding a New Feature (Service Module) End-to-End

This section walks through adding a new feature "Products" with full CRUD operations. **Remember the architectural rules**: Services cannot reference other services; only repositories can be shared across services.

### Step 1: Entity Model

Add `libraries/DemoProject.Data/Models/Product.cs`:

```csharp
namespace DemoProject.Data.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}
```

### Step 2: Table Configuration and Migration

Add `libraries/DemoProject.Data/TableConfigurations/ProductTableConfiguration.cs`:

```csharp
using DemoProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductTableConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "dbo");
        builder.HasKey(e => e.ProductId);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Price).HasColumnType("decimal(18,2)");
    }
}
```

Add DbSet to `Data/DbContext/ApplicationDbContext.cs`:

```csharp
public DbSet<Product> Products { get; set; }
```

Then create and apply migration from the Data project directory:

```powershell
cd .\src\libraries\DemoProject.Data\
dotnet ef migrations add AddProductTable --context ApplicationDbContext --output-dir Migrations --startup-project ../../applications/DemoProject.WebApi
dotnet ef database update --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi
```

### Step 3: Repository Interface and Implementation

**Interface** (`Data/Repository/Interfaces/IProductRepository.cs`):

```csharp
using DemoProject.Data.Models;
using DemoProject.Data.Repository.Interfaces.Base;

public interface IProductRepository : IRepositoryWithSearch<Product, int, ProductSearchFilters>
{
}

public class ProductSearchFilters
{
    public string Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
```

**Implementation** (`Data/Repository/Implementation/ProductRepository.cs`):

```csharp
public class ProductRepository : BaseRepositoryWithSearch<Product, int, ProductSearchFilters>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductSearchFilters filters)
    {
        if (!string.IsNullOrWhiteSpace(filters?.Name))
            query = query.Where(p => p.Name.Contains(filters.Name));

        if (filters?.MinPrice.HasValue == true)
            query = query.Where(p => p.Price >= filters.MinPrice.Value);

        if (filters?.MaxPrice.HasValue == true)
            query = query.Where(p => p.Price <= filters.MaxPrice.Value);

        return query;
    }
}
```

Register in DI (`Data/DependencyInjectionDependencies.cs`):

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

### Step 4: DTOs (Requests/Responses)

Create folder `libraries/DemoProject.Services/Modules/Product/` with subfolders:

- `Requests/`
- `Responses/`

**Requests** (`Modules/Product/Requests/`):

- `CreateProductRequest.cs` (with validation: `[Required]`, `[StringLength]`, etc.)
- `UpdateProductRequest.cs` (includes `ProductId` + inherits validation from Create)
- `GetProductByIdRequest.cs`
- `GetAllProductsRequest.cs`
- `SearchProductsRequest.cs` (with `ProductSearchFilters` property)
- `DeleteProductRequest.cs`

**Responses** (`Modules/Product/Responses/`):

- `Product.cs` (DTO model matching entity properties needed by API consumers)
- `GetProductByIdResponse.cs` (contains single `Product`)
- `GetAllProductsResponse.cs` (contains list of `Product`)
- `SearchProductsResponse.cs` (inherit from shared `SearchResponse<Product>`)
- `CreateProductResponse.cs` (contains created `Product`)
- `UpdateProductResponse.cs` (contains updated `Product`)
- `DeleteProductResponse.cs` (contains success indicator)

### Step 5: Mapping Configuration

Add `ProductMappingConfiguration.cs` in `Modules/Product/`:

```csharp
using Mapster;

namespace DemoProject.Services.Modules.Product;

public static class ProductMappingConfiguration
{
    public static void ConfigureMappings()
    {
        // Entity to DTO
        TypeAdapterConfig<Data.Models.Product, Responses.Product>.NewConfig();

        // Requests to Entity
        TypeAdapterConfig<Requests.CreateProductRequest, Data.Models.Product>.NewConfig();
        TypeAdapterConfig<Requests.UpdateProductRequest, Data.Models.Product>.NewConfig();
    }
}
```

### Step 6: Service Interface and Implementation

**Interface** (`Modules/Product/IProductService.cs`):

```csharp
using DemoProject.Services.Modules.Product.Requests;
using DemoProject.Services.Modules.Product.Responses;

namespace DemoProject.Services.Modules.Product;

public interface IProductService
{
    Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request);
    Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request);
    Task<SearchProductsResponse> SearchProducts(SearchProductsRequest request);
    Task<CreateProductResponse> CreateProduct(CreateProductRequest request);
    Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request);
    Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request);
}
```

**Implementation** (`Modules/Product/ProductService.cs`):

```csharp
using DemoProject.Data.Repository.Interfaces;
using DemoProject.Services.Shared.Exceptions;
using Mapster;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<CreateProductResponse> CreateProduct(CreateProductRequest request)
    {
        var entity = request.Adapt<Data.Models.Product>();
        await _productRepository.Create(entity);
        return new CreateProductResponse { Product = entity.Adapt<Responses.Product>() };
    }

    public async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request)
    {
        var entity = await _productRepository.GetById(request.Id);
        if (entity == null)
            throw new NotFoundException($"Product with ID {request.Id} not found.");

        return new GetProductByIdResponse { Product = entity.Adapt<Responses.Product>() };
    }

    // Implement remaining methods (GetAll, Search, Update, Delete) following DemoService pattern
}
```

Register in DI (`Services/DependencyInjectionExtensions.cs`):

```csharp
services.AddScoped<IProductService, ProductService>();
ProductMappingConfiguration.ConfigureMappings();
```

### Step 7: Controller

Add `ProductsController.cs` in `applications/DemoProject.WebApi/Controllers/`:

```csharp
using DemoProject.Services.Modules.Product;
using DemoProject.Services.Modules.Product.Requests;
using DemoProject.Services.Modules.Product.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DemoProject.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductByIdResponse>> GetProductById(int id)
    {
        var response = await _productService.GetProductById(new GetProductByIdRequest { Id = id });
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetAllProductsResponse>> GetAllProducts()
    {
        var response = await _productService.GetAllProducts(new GetAllProductsRequest());
        return Ok(response);
    }

    [HttpPost("search")]
    public async Task<ActionResult<SearchProductsResponse>> SearchProducts([FromBody] SearchProductsRequest request)
    {
        var response = await _productService.SearchProducts(request);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var response = await _productService.CreateProduct(request);
        return CreatedAtAction(nameof(GetProductById), new { id = response.Product.ProductId }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateProductResponse>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        request.ProductId = id;
        var response = await _productService.UpdateProduct(request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteProductResponse>> DeleteProduct(int id)
    {
        var response = await _productService.DeleteProduct(new DeleteProductRequest { Id = id });
        return Ok(response);
    }
}
```

### Step 8: Verify and Test

- Run the application and test via Swagger UI in Development mode
- Create HTTP file in `HttpFiles/ProductController.http` for manual testing
- Verify database schema changes in SQL Server

### Key Architectural Reminders

- ✅ **One controller → one service (1:1)**: `ProductsController` uses only `IProductService`
- ✅ **Services cannot reference other services**: `ProductService` cannot inject `DemoService`
- ✅ **Repositories can be shared**: Multiple services can use `IProductRepository`
- ✅ **Service modules reference only common modules**: Data and Infrastructure only

## Request/Response Standards

> Every service operation in the service layer must define explicit Request and Response classes. Each method in a service interface accepts a Request and returns a Response. Do not pass primitives or entity models directly across the service boundary.

- **Placement**: Feature-specific types live under `libraries/DemoProject.Services/Modules/<Feature>/Requests` and `.../Responses`.
- **Shared Models**: Common types like `SearchRequest<T>` and `SearchResponse<T>` live in `Services/Shared/Models/`.
- **Reuse Strategies**:
  - Use composition by including shared models (e.g., `SearchRequest<ProductSearchFilters>`)
  - Use inheritance for common validation (e.g., `UpdateProductRequest` can inherit from `CreateProductRequest` and add `ProductId`)
- **Naming**: `<Operation><Feature>Request` and `<Operation><Feature>Response` (e.g., `GetDemoByIdRequest`, `CreateProductResponse`)
- **Validation**: Put data annotations (e.g., `[Required]`, `[StringLength]`, `[Range]`) on Request types, **not** on entity models

## Mapster Mapping Standards

- Define mappings in `<Feature>MappingConfiguration.cs` within each feature module
- Keep mappings 1:1 between DTOs and entities
- Register all mappings in `Services.AddApplicationServices()` by calling each `ConfigureMappings()` method
- In service methods, use `.Adapt<T>()` for conversions
- Example:
  ```csharp
  var entity = request.Adapt<Data.Models.Product>();
  var dto = entity.Adapt<Responses.Product>();
  ```

## Transactions and Unit of Work

- By default, repository methods auto-commit changes
- For multi-step operations requiring atomicity, inject `IUnitOfWork`:

  ```csharp
  public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
  {
      _productRepository = productRepository;
      _unitOfWork = unitOfWork;
  }

  // Usage:
  await _unitOfWork.BeginTransaction();
  try
  {
      await _productRepository.Create(product);
      // ... more operations
      await _unitOfWork.CommitTransaction();
  }
  catch
  {
      await _unitOfWork.RollbackTransaction();
      throw;
  }
  ```

## Error Handling

- Throw business exceptions from Services (defined in `Services/Shared/Exceptions/`):
  - `NotFoundException` for missing entities
  - `ValidationException` for invalid requests or business rule violations
  - `UnauthorizedException` for authorization failures
  - `UnexpectedErrorException` for unexpected system errors
- API layer converts all exceptions to RFC7807 ProblemDetails via `GlobalExceptionHandlerMiddleware`
- HTTP status codes are automatically set: 404 for NotFound, 400 for Validation, 401 for Unauthorized, 500 for Unexpected

## Adding DI for New Components

### Adding a New Service

Register in `Services/DependencyInjectionExtensions.cs` → `AddApplicationServices()`:

```csharp
services.AddScoped<IProductService, ProductService>();
ProductMappingConfiguration.ConfigureMappings();
```

### Adding a New Repository

Register in `Data/DependencyInjectionDependencies.cs` → `AddRepositoryServices()`:

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

### Adding a New Infrastructure Service

Register in `Infrastructure/DependencyInjectionExtensions.cs` → `AddInfrastructureServices()`:

```csharp
services.AddScoped<IEmailService, EmailService>();
```

### Wiring in Program.cs

All extension methods are already wired:

```csharp
builder.Services.AddInfrastructureServices();  // Infrastructure common module
builder.Services.AddRepositoryServices();       // Data common module
builder.Services.AddApplicationServices();      // Services module
```

## Swagger and HTTP Files

- Swagger UI is enabled in Development mode and accessible at `/swagger`
- Sample HTTP requests for manual testing are in `applications/DemoProject.WebApi/HttpFiles/`
- Each controller should have a corresponding `.http` file (e.g., `DemoController.http`)

## Checklist for Adding a New Feature

- ✅ Entity model added in `DemoProject.Data/Models/`
- ✅ Table configuration created in `Data/TableConfigurations/`
- ✅ DbSet added to `ApplicationDbContext`
- ✅ Migration generated and applied from Data project
- ✅ Repository interface and implementation added in `Data/Repository/`
- ✅ Repository registered in `Data/DependencyInjectionDependencies.cs`
- ✅ Service module folder created under `Services/Modules/<Feature>/`
- ✅ Requests and Responses defined in service module
- ✅ Mapping configuration created and registered
- ✅ Service interface and implementation added
- ✅ Service registered in `Services/DependencyInjectionExtensions.cs`
- ✅ Controller added in `WebApi/Controllers/`
- ✅ Manual tests via Swagger or `.http` file
- ✅ Consider adding unit tests for business logic

## EF Core Commands Quick Reference (Windows PowerShell)

**Important**: Run these commands from the `libraries/DemoProject.Data` directory:

```powershell
# Navigate to Data project
cd .\src\libraries\DemoProject.Data\

# Add a new migration
dotnet ef migrations add <MigrationName> --context ApplicationDbContext --output-dir Migrations --startup-project ../../applications/DemoProject.WebApi

# Apply migrations to database
dotnet ef database update --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi

# Rollback to a specific migration
dotnet ef database update <MigrationName> --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi

# Remove last migration (if not applied)
dotnet ef migrations remove --context ApplicationDbContext --startup-project ../../applications/DemoProject.WebApi
```
