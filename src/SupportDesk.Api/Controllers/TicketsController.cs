using Microsoft.AspNetCore.Mvc;
using SupportDesk.Application.Common;
using SupportDesk.Application.Tickets;
using SupportDesk.Application.Tickets.Dtos;

namespace SupportDesk.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets)
    {
        _tickets = tickets;
    }

    [HttpGet]
    public async Task<ActionResult<TicketListDto>> List(
        [FromQuery] TicketListQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.ListAsync(query, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailList(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.GetByIdAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailDetail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<TicketDetailDto>> Create(
        [FromBody] CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.CreateAsync(request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailDetail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> Update(
        Guid id,
        [FromBody] UpdateTicketRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.UpdateAsync(id, request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailDetail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BooleanDto>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.DeleteAsync(id, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(new BooleanDto { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<ActionResult<TicketDetailDto>> Assign(
        Guid id,
        [FromBody] AssignAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.AssignAsync(id, request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailDetail(ex.Message));
        }
    }

    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<TicketDetailDto>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.ChangeStatusAsync(id, request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailDetail(ex.Message));
        }
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<TicketDetailDto>> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _tickets.AddCommentAsync(id, request, cancellationToken));
        }
        catch (Exception ex)
        {
            return Ok(TicketMapper.FailDetail(ex.Message));
        }
    }
}
