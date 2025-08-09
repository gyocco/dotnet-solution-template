using DemoProject.Services.Demo;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProject.Services;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
  {
    services.AddScoped<IDemoService, DemoService>();

    DemoMappingConfiguration.ConfigureMappings();

    return services;
  }
}
