using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Agents;
using SupportDesk.Application.Agents.Dtos;
using SupportDesk.Application.Common;
using SupportDesk.Domain.Entities;
using SupportDesk.Infrastructure.Persistence;

namespace SupportDesk.Infrastructure.Services;

public class AgentService : IAgentService
{
    private readonly AppDbContext _db;

    public AgentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AgentListDto> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = _db.Agents.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a => a.FullName.Contains(term) || a.Email.Contains(term));
        }

        var agents = await query.OrderBy(a => a.FullName).ToListAsync(cancellationToken);
        return new AgentListDto
        {
            Success = true,
            Message = "Agents loaded.",
            Items = agents.Select(AgentMapper.ToListItem).ToList()
        };
    }

    public async Task<AgentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var agent = await _db.Agents.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (agent is null)
        {
            return AgentMapper.Fail("Agent was not found.");
        }

        return AgentMapper.ToDto(agent, "Agent loaded.");
    }

    public async Task<AgentDto> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken)
    {
        if (await EmailTakenAsync(request.Email.Trim(), null, cancellationToken))
        {
            return AgentMapper.Fail("An agent with this email already exists.");
        }

        var agent = new Agent(
            Guid.NewGuid(),
            request.FullName,
            request.Email,
            request.Department,
            request.IsActive);

        _db.Agents.Add(agent);
        await _db.SaveChangesAsync(cancellationToken);
        return AgentMapper.ToDto(agent, "Agent created.");
    }

    public async Task<AgentDto> UpdateAsync(Guid id, UpdateAgentRequest request, CancellationToken cancellationToken)
    {
        var agent = await _db.Agents.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (agent is null)
        {
            return AgentMapper.Fail("Agent was not found.");
        }

        if (await EmailTakenAsync(request.Email.Trim(), id, cancellationToken))
        {
            return AgentMapper.Fail("An agent with this email already exists.");
        }

        agent.Update(request.FullName, request.Email, request.Department);
        await _db.SaveChangesAsync(cancellationToken);
        return AgentMapper.ToDto(agent, "Agent updated.");
    }

    public async Task<BooleanDto> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var agent = await _db.Agents.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (agent is null)
        {
            return new BooleanDto { Success = false, Message = "Agent was not found." };
        }

        var hasTickets = await _db.Tickets.AnyAsync(t => t.AgentId == id, cancellationToken);
        if (hasTickets)
        {
            return new BooleanDto
            {
                Success = false,
                Message = "Cannot delete an agent who is assigned to one or more tickets. Reassign or unassign those tickets first."
            };
        }

        _db.Agents.Remove(agent);
        await _db.SaveChangesAsync(cancellationToken);
        return new BooleanDto { Success = true, Message = "Agent deleted." };
    }

    public async Task<AgentDto> SetActiveAsync(Guid id, SetAgentActiveRequest request, CancellationToken cancellationToken)
    {
        var agent = await _db.Agents.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (agent is null)
        {
            return AgentMapper.Fail("Agent was not found.");
        }

        if (request.IsActive)
        {
            agent.Activate();
        }
        else
        {
            agent.Deactivate();
        }

        await _db.SaveChangesAsync(cancellationToken);
        return AgentMapper.ToDto(agent, request.IsActive ? "Agent activated." : "Agent deactivated.");
    }

    private async Task<bool> EmailTakenAsync(string email, Guid? excludingId, CancellationToken cancellationToken)
    {
        return await _db.Agents.AnyAsync(
            a => a.Email == email && (!excludingId.HasValue || a.Id != excludingId.Value),
            cancellationToken);
    }
}
