using DemoProject.Domain.Entities;
using DemoProject.Services.Demo.Requests;
using Mapster;

namespace DemoProject.Services.Demo;

public static class DemoMappingConfiguration
{
  public static void ConfigureMappings()
  {
    TypeAdapterConfig<DemoEntity, _Shared.Responses.Demo>.NewConfig();
    TypeAdapterConfig<CreateDemoRequest, DemoEntity>.NewConfig();
    TypeAdapterConfig<UpdateDemoRequest, DemoEntity>.NewConfig();
  }
}
