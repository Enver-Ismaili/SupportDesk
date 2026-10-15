using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Common;
using SupportDesk.Application.Tickets;
using SupportDesk.Application.Tickets.Dtos;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Infrastructure.Persistence;

namespace SupportDesk.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;
    private readonly TicketReferenceGenerator _references;

    public TicketService(AppDbContext db, TicketReferenceGenerator references)
    {
        _db = db;
        _references = references;
    }

    public async Task<TicketListDto> ListAsync(TicketListQuery query, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var tickets = _db.Tickets.AsNoTracking().Include(t => t.Agent).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            tickets = tickets.Where(t =>
                t.Reference.Contains(term) ||
                t.Title.Contains(term) ||
                t.CustomerName.Contains(term));
        }

        if (query.Status is not null)
        {
            tickets = tickets.Where(t => t.Status == query.Status);
        }

        if (query.Priority is not null)
        {
            tickets = tickets.Where(t => t.Priority == query.Priority);
        }

        if (query.AgentId is not null)
        {
            tickets = tickets.Where(t => t.AgentId == query.AgentId);
        }

        if (query.OverdueOnly)
        {
            tickets = tickets.Where(t =>
                t.DueAt < now &&
                t.Status != TicketStatus.Resolved &&
                t.Status != TicketStatus.Closed);
        }

        var total = await tickets.CountAsync(cancellationToken);
        var items = await tickets
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new TicketListDto
        {
            Success = true,
            Message = "Tickets loaded.",
            Items = items.Select(t => TicketMapper.ToListItem(t, now)).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TicketDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await FindAsync(id, cancellationToken, asNoTracking: true);
        if (ticket is null)
        {
            return TicketMapper.FailDetail("Ticket was not found.");
        }

        return TicketMapper.ToDetail(ticket, DateTime.UtcNow, "Ticket loaded.");
    }

    public async Task<TicketDetailDto> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        var reference = await _references.NextAsync(_db, now, cancellationToken);
        var ticket = Ticket.Create(
            Guid.NewGuid(),
            reference,
            request.Title.Trim(),
            request.Description.Trim(),
            request.CustomerName.Trim(),
            request.CustomerEmail.Trim(),
            request.Priority,
            now);

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return TicketMapper.ToDetail(ticket, now, "Ticket created.");
    }

    public async Task<TicketDetailDto> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await FindAsync(id, cancellationToken);
        if (ticket is null)
        {
            return TicketMapper.FailDetail("Ticket was not found.");
        }

        var result = ticket.UpdateDetails(
            request.Title.Trim(),
            request.Description.Trim(),
            request.CustomerName.Trim(),
            request.CustomerEmail.Trim(),
            request.Priority,
            DateTime.UtcNow);

        if (!result.Success)
        {
            return TicketMapper.FailDetail(result.Message);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return TicketMapper.ToDetail(ticket, DateTime.UtcNow, result.Message);
    }

    public async Task<BooleanDto> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var ticket = await FindAsync(id, cancellationToken);
        if (ticket is null)
        {
            return new BooleanDto { Success = false, Message = "Ticket was not found." };
        }

        if (ticket.Status == TicketStatus.Closed)
        {
            return new BooleanDto { Success = false, Message = "A closed ticket is read-only and cannot be deleted." };
        }

        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync(cancellationToken);
        return new BooleanDto { Success = true, Message = "Ticket deleted." };
    }

    public async Task<TicketDetailDto> AssignAsync(Guid id, AssignAgentRequest request, CancellationToken cancellationToken)
    {
        var ticket = await FindAsync(id, cancellationToken);
        if (ticket is null)
        {
            return TicketMapper.FailDetail("Ticket was not found.");
        }

        var now = DateTime.UtcNow;
        if (request.AgentId is null)
        {
            var unassign = ticket.Unassign(now);
            if (!unassign.Success)
            {
                return TicketMapper.FailDetail(unassign.Message);
            }
        }
        else
        {
            var agent = await _db.Agents.FirstOrDefaultAsync(a => a.Id == request.AgentId, cancellationToken);
            if (agent is null)
            {
                return TicketMapper.FailDetail("Agent was not found.");
            }

            var assign = ticket.Assign(agent, now);
            if (!assign.Success)
            {
                return TicketMapper.FailDetail(assign.Message);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return TicketMapper.ToDetail(ticket, now, "Assignment updated.");
    }

    public async Task<TicketDetailDto> ChangeStatusAsync(Guid id, ChangeStatusRequest request, CancellationToken cancellationToken)
    {
        var ticket = await FindAsync(id, cancellationToken);
        if (ticket is null)
        {
            return TicketMapper.FailDetail("Ticket was not found.");
        }

        var result = ticket.ChangeStatus(request.Status, DateTime.UtcNow);
        if (!result.Success)
        {
            return TicketMapper.FailDetail(result.Message);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return TicketMapper.ToDetail(ticket, DateTime.UtcNow, result.Message);
    }

    public async Task<TicketDetailDto> AddCommentAsync(Guid id, AddCommentRequest request, CancellationToken cancellationToken)
    {
        var ticket = await FindAsync(id, cancellationToken);
        if (ticket is null)
        {
            return TicketMapper.FailDetail("Ticket was not found.");
        }

        var result = ticket.AddComment(request.AuthorName.Trim(), request.Body.Trim(), DateTime.UtcNow);
        if (!result.Success)
        {
            return TicketMapper.FailDetail(result.Message);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return TicketMapper.ToDetail(ticket, DateTime.UtcNow, result.Message);
    }

    private async Task<Ticket?> FindAsync(Guid id, CancellationToken cancellationToken, bool asNoTracking = false)
    {
        IQueryable<Ticket> query = _db.Tickets
            .Include(t => t.Agent)
            .Include(t => t.Comments);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}
