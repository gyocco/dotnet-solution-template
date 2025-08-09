using DemoProject.Services.Demo;
using DemoProject.Services.Demo.Requests;
using DemoProject.Services.Demo.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DemoProject.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemosController : ControllerBase
{
  private readonly IDemoService _demoService;

  public DemosController(IDemoService demoService)
  {
    _demoService = demoService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<GetDemoByIdResponse>> GetDemoById(int id)
  {
    var response = await _demoService.GetDemoById(new GetDemoByIdRequest { Id = id });
    return Ok(response);
  }

  [HttpGet]
  public async Task<ActionResult<GetAllDemosResponse>> GetAllDemos()
  {
    var response = await _demoService.GetAllDemos(new GetAllDemosRequest());
    return Ok(response);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchDemosResponse>> SearchDemos([FromBody] SearchDemosRequest request)
  {
    var response = await _demoService.SearchDemos(request);
    return Ok(response);
  }

  [HttpPost]
  public async Task<ActionResult<CreateDemoResponse>> CreateDemo([FromBody] CreateDemoRequest request)
  {
    var response = await _demoService.CreateDemo(request);
    return CreatedAtAction(nameof(GetDemoById), new { id = response.Demo.DemoId }, response);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<UpdateDemoResponse>> UpdateDemo(int id, [FromBody] UpdateDemoRequest request)
  {
    request.DemoId = id;
    var response = await _demoService.UpdateDemo(request);
    return Ok(response);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<DeleteDemoResponse>> DeleteDemo(int id)
  {
    var response = await _demoService.DeleteDemo(new DeleteDemoRequest { Id = id });
    return Ok(response);
  }
}
