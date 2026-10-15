using SupportDesk.Application.Common;
using SupportDesk.Application.Tickets.Dtos;

namespace SupportDesk.Application.Tickets;

public interface ITicketService
{
    Task<TicketListDto> ListAsync(TicketListQuery query, CancellationToken cancellationToken);
    Task<TicketDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TicketDetailDto> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken);
    Task<TicketDetailDto> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken);
    Task<BooleanDto> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<TicketDetailDto> AssignAsync(Guid id, AssignAgentRequest request, CancellationToken cancellationToken);
    Task<TicketDetailDto> ChangeStatusAsync(Guid id, ChangeStatusRequest request, CancellationToken cancellationToken);
    Task<TicketDetailDto> AddCommentAsync(Guid id, AddCommentRequest request, CancellationToken cancellationToken);
}
