using DemoProject.Services.Modules.Demo.Requests;
using DemoProject.Data.Repository.Interfaces.Base.Models;
using Mapster;

namespace DemoProject.Services.Modules.Demo;

public static class DemoMappingConfiguration
{
  public static void ConfigureMappings()
  {
    TypeAdapterConfig<Data.Models.Demo, Responses.Demo>.NewConfig();
    TypeAdapterConfig<CreateDemoRequest, Data.Models.Demo>.NewConfig();
    TypeAdapterConfig<UpdateDemoRequest, Data.Models.Demo>.NewConfig();
    TypeAdapterConfig<SearchDemosRequest, SearchRequest<SearchDemoFilters>>.NewConfig();
  }
}
