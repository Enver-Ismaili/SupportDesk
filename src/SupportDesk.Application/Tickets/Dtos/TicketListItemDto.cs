using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Dtos;

public class TicketListItemDto
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public Guid? AgentId { get; set; }
    public string? AgentName { get; set; }
    public DateTime DueAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsOverdue { get; set; }
}
