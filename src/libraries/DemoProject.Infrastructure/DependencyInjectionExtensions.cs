using Microsoft.Extensions.DependencyInjection;

namespace DemoProject.Infrastructure;

public static class DependencyInjectionDependencies
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // TODO: Add infrastructure services here, e.g., azure storage, email service, etc.
        return services;
    }
}