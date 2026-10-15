using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Dtos;

public record CreateTicketRequest(
    string Title,
    string Description,
    string CustomerName,
    string CustomerEmail,
    Priority Priority);
