using SupportDesk.Application.Common;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Dtos;

public class TicketDetailDto : BooleanDto
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public Guid? AgentId { get; set; }
    public string? AgentName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime DueAt { get; set; }
    public bool IsOverdue { get; set; }
    public IReadOnlyList<TicketStatus> AllowedStatusTargets { get; set; } = [];
    public IReadOnlyList<CommentDto> Comments { get; set; } = [];
}
