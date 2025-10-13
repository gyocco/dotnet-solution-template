using DemoProject.Services.Modules.Demo.Requests;
using DemoProject.Services.Modules.Demo.Responses;
using DemoProject.Services.Shared.Exceptions;
using DemoProject.Data.Repository.Interfaces;
using DemoProject.Data.Repository.Interfaces.Base.Models;
using Mapster;

namespace DemoProject.Services.Modules.Demo;

public class DemoService : IDemoService
{
    private readonly IDemoRepository _demoRepository;

    public DemoService(IDemoRepository demoRepository)
    {
        _demoRepository = demoRepository;
    }

    public async Task<CreateDemoResponse> CreateDemo(CreateDemoRequest request)
    {
        var demoEntity = request.Adapt<Data.Models.Demo>();
        await _demoRepository.Create(demoEntity);

        var demoResponse = demoEntity.Adapt<Responses.Demo>();
        return new CreateDemoResponse { Demo = demoResponse };
    }

    public async Task<DeleteDemoResponse> DeleteDemo(DeleteDemoRequest request)
    {
        var demoEntity = await _demoRepository.GetById(request.Id);
        if (demoEntity == null)
        {
            throw new NotFoundException($"Demo with ID {request.Id} not found.");
        }

        await _demoRepository.Delete(demoEntity);
        return new DeleteDemoResponse { Success = true };
    }

    public async Task<GetAllDemosResponse> GetAllDemos(GetAllDemosRequest request)
    {
        var demoEntities = await _demoRepository.GetAll();
        var demoResponses = demoEntities.Adapt<List<Responses.Demo>>();

        return new GetAllDemosResponse { Demos = demoResponses };
    }

    public async Task<GetDemoByIdResponse> GetDemoById(GetDemoByIdRequest request)
    {
        var demoEntity = await _demoRepository.GetById(request.Id);
        if (demoEntity == null)
        {
            throw new NotFoundException($"Demo with ID {request.Id} not found.");
        }

        var demoResponse = demoEntity.Adapt<Responses.Demo>();
        return new GetDemoByIdResponse { Demo = demoResponse };
    }

    public async Task<SearchDemosResponse> SearchDemos(SearchDemosRequest request)
    {
        var searchRequest = request.Adapt<SearchRequest<DemoSearchFilters>>();
        searchRequest.Filters = request.Filters.Adapt<DemoSearchFilters>();
        var searchResult = await _demoRepository.Search(searchRequest);
        var demoResponses = searchResult.Items.Adapt<List<Responses.Demo>>();

        return new SearchDemosResponse
        {
            Results = demoResponses,
            PageNumber = searchResult.PageNumber,
            PageSize = searchResult.PageSize,
            TotalItems = searchResult.TotalCount,
            TotalPages = searchResult.TotalPages
        };
    }

    public async Task<UpdateDemoResponse> UpdateDemo(UpdateDemoRequest request)
    {
        var existingDemo = await _demoRepository.GetById(request.DemoId);
        if (existingDemo == null)
        {
            throw new NotFoundException($"Demo with ID {request.DemoId} not found.");
        }

        // Map the request to the existing entity
        request.Adapt(existingDemo);
        await _demoRepository.Update(existingDemo);

        var demoResponse = existingDemo.Adapt<Responses.Demo>();
        return new UpdateDemoResponse { Demo = demoResponse };
    }
}