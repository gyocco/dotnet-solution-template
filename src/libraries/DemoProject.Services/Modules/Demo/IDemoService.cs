using DemoProject.Services.Modules.Demo.Requests;
using DemoProject.Services.Modules.Demo.Responses;

namespace DemoProject.Services.Modules.Demo;

public interface IDemoService
{
  Task<GetDemoByIdResponse> GetDemoById(GetDemoByIdRequest request);
  Task<GetAllDemosResponse> GetAllDemos(GetAllDemosRequest request);
  Task<SearchDemosResponse> SearchDemos(SearchDemosRequest request);
  Task<CreateDemoResponse> CreateDemo(CreateDemoRequest request);
  Task<UpdateDemoResponse> UpdateDemo(UpdateDemoRequest request);
  Task<DeleteDemoResponse> DeleteDemo(DeleteDemoRequest request);
}
