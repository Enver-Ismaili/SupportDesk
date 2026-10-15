using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Dtos;

public record TicketListQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    TicketStatus? Status = null,
    Priority? Priority = null,
    Guid? AgentId = null,
    bool OverdueOnly = false);
