using DemoProject.Data.Repository.Interfaces;
using DemoProject.Data.Repository.Interfaces.Base;
using DemoProject.Data.Repository.Implementation;
using DemoProject.Data.Repository.Implementation.Base;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProject.Data;

public static class DependencyInjectionDependencies
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDemoRepository, DemoRepository>();

        return services;
    }
}