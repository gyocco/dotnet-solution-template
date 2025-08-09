using DemoProject.Infrastructure.Data;
using DemoProject.Services._Shared.InfrastructureInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProject.Infrastructure;

public static class InfrastructureCollectionExtensions
{
  public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
  {
    services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));

    return services;
  }
}
