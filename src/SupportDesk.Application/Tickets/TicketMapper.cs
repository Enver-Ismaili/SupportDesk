using SupportDesk.Application.Tickets.Dtos;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Tickets;

public static class TicketMapper
{
    public static TicketListItemDto ToListItem(Ticket ticket, DateTime utcNow)
    {
        return new TicketListItemDto
        {
            Id = ticket.Id,
            Reference = ticket.Reference,
            Title = ticket.Title,
            CustomerName = ticket.CustomerName,
            Priority = ticket.Priority,
            Status = ticket.Status,
            AgentId = ticket.AgentId,
            AgentName = ticket.Agent?.FullName,
            DueAt = ticket.DueAt,
            CreatedAt = ticket.CreatedAt,
            IsOverdue = ticket.IsOverdue(utcNow)
        };
    }

    public static TicketDetailDto ToDetail(Ticket ticket, DateTime utcNow, string message = "OK")
    {
        var comments = ticket.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                AuthorName = c.AuthorName,
                Body = c.Body,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return new TicketDetailDto
        {
            Success = true,
            Message = message,
            Id = ticket.Id,
            Reference = ticket.Reference,
            Title = ticket.Title,
            Description = ticket.Description,
            CustomerName = ticket.CustomerName,
            CustomerEmail = ticket.CustomerEmail,
            Priority = ticket.Priority,
            Status = ticket.Status,
            AgentId = ticket.AgentId,
            AgentName = ticket.Agent?.FullName,
            CreatedAt = ticket.CreatedAt,
            LastModifiedAt = ticket.LastModifiedAt,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt,
            DueAt = ticket.DueAt,
            IsOverdue = ticket.IsOverdue(utcNow),
            AllowedStatusTargets = ticket.AllowedStatusTargets(),
            Comments = comments
        };
    }

    public static TicketDetailDto FailDetail(string message)
    {
        return new TicketDetailDto
        {
            Success = false,
            Message = message
        };
    }

    public static TicketListDto FailList(string message)
    {
        return new TicketListDto
        {
            Success = false,
            Message = message
        };
    }
}
