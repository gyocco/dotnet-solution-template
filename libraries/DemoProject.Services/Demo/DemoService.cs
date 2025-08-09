using DemoProject.Domain.Exceptions;
using DemoProject.Services._Shared.InfrastructureInterfaces;
using DemoProject.Services.Demo.Requests;
using DemoProject.Services.Demo.Responses;
using Mapster;

namespace DemoProject.Services.Demo;

public class DemoService(IDataRepository<Domain.Entities.DemoEntity> demoRepository) : IDemoService
{
  private readonly IDataRepository<Domain.Entities.DemoEntity> _demoRepository = demoRepository;

  public async Task<GetDemoByIdResponse> GetDemoById(GetDemoByIdRequest request)
  {
    var demo = await _demoRepository.GetById(request.Id);
    return new GetDemoByIdResponse
    {
      Demo = demo?.Adapt<_Shared.Responses.Demo>()
    };
  }

  public async Task<GetAllDemosResponse> GetAllDemos(GetAllDemosRequest request)
  {
    var demos = await _demoRepository.GetAll();
    var demosDto = demos.Adapt<List<_Shared.Responses.Demo>>();
    return new GetAllDemosResponse
    {
      Demos = demosDto
    };
  }

  public async Task<SearchDemosResponse> SearchDemos(SearchDemosRequest request)
  {
    var searchResult = await _demoRepository.Search(
      filter: string.IsNullOrEmpty(request.Query) ? null : demo => demo.Name.Contains(request.Query),
      orderBy: GetOrderByExpression(request.OrderBy),
      orderByDescending: request.OrderByDescending,
      pageNumber: request.PageNumber,
      pageSize: request.PageSize
    );

    var demosDto = searchResult.Items.Adapt<List<_Shared.Responses.Demo>>();

    return new SearchDemosResponse
    {
      Results = demosDto,
      PageNumber = searchResult.PageNumber,
      PageSize = searchResult.PageSize,
      TotalItems = searchResult.TotalItems,
      TotalPages = searchResult.TotalPages
    };
  }

  public async Task<CreateDemoResponse> CreateDemo(CreateDemoRequest request)
  {
    var demo = request.Adapt<Domain.Entities.DemoEntity>();
    await _demoRepository.Add(demo);
    return new CreateDemoResponse
    {
      Demo = demo.Adapt<_Shared.Responses.Demo>()
    };
  }

  public async Task<UpdateDemoResponse> UpdateDemo(UpdateDemoRequest request)
  {
    var existingDemo = await _demoRepository.GetById(request.DemoId);

    if (existingDemo == null)
    {
      throw new NotFoundException($"Demo with ID {request.DemoId} not found.");
    }

    existingDemo = request.Adapt(existingDemo);
    await _demoRepository.Update(existingDemo);

    return new UpdateDemoResponse
    {
      Demo = existingDemo.Adapt<_Shared.Responses.Demo>()
    };
  }

  public async Task<DeleteDemoResponse> DeleteDemo(DeleteDemoRequest request)
  {
    var demo = await _demoRepository.GetById(request.Id);

    if (demo == null)
    {
      throw new NotFoundException($"Demo with ID {request.Id} not found.");
    }

    await _demoRepository.Remove(demo);

    return new DeleteDemoResponse { Success = true };
  }

  private System.Linq.Expressions.Expression<Func<Domain.Entities.DemoEntity, object>> GetOrderByExpression(string orderBy)
  {
    return orderBy?.ToLower() switch
    {
      "name" => demo => demo.Name,
      "id" or "demoid" => demo => demo.DemoId,
      _ => demo => demo.DemoId
    };
  }
}
