using DemoProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DemoProject.Services;
using DemoProject.Infrastructure;
using DemoProject.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("DemoProject.Infrastructure")));

builder.Services.AddServiceDependencies();
builder.Services.AddInfrastructureDependencies();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Add global exception handler middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
