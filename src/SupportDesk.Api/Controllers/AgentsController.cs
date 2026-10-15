using Microsoft.AspNetCore.Mvc;
using SupportDesk.Application.Agents;
using SupportDesk.Application.Agents.Dtos;
using SupportDesk.Application.Common;

namespace SupportDesk.Api.Controllers;

[ApiController]
[Route("api/agents")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agents;

    public AgentsController(IAgentService agents)
    {
        _agents = agents;
    }

    [HttpGet]
    public async Task<ActionResult<AgentListDto>> List(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _agents.ListAsync(search, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(AgentMapper.FailList(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AgentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _agents.GetByIdAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(AgentMapper.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<AgentDto>> Create(
        [FromBody] CreateAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _agents.CreateAsync(request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(AgentMapper.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AgentDto>> Update(
        Guid id,
        [FromBody] UpdateAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _agents.UpdateAsync(id, request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(AgentMapper.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BooleanDto>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _agents.DeleteAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(new BooleanDto { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/active")]
    public async Task<ActionResult<AgentDto>> SetActive(
        Guid id,
        [FromBody] SetAgentActiveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _agents.SetActiveAsync(id, request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(AgentMapper.Fail(ex.Message));
        }
    }
}
