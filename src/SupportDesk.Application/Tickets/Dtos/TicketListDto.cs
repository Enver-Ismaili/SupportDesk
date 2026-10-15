using SupportDesk.Application.Common;

namespace SupportDesk.Application.Tickets.Dtos;

public class TicketListDto : BooleanDto
{
    public IReadOnlyList<TicketListItemDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
