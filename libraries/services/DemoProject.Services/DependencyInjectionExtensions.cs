using DemoProject.Services.Modules.Demo;
using Microsoft.Extensions.DependencyInjection;

namespace DemoProject.Services;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<IDemoService, DemoService>();

    DemoMappingConfiguration.ConfigureMappings();

    return services;
  }
}
