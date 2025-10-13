using DemoProject.Data.Repository;
using DemoProject.Data.Repository.Base;
using DemoProject.Data.Implementation.Repository;
using DemoProject.Data.Implementation.Repository.Base;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProject.Data.Implementation;

public static class DependencyInjectionDependencies
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDemoRepository, DemoRepository>();

        return services;
    }
}