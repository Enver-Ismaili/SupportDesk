using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Dtos;

public record UpdateTicketRequest(
    string Title,
    string Description,
    string CustomerName,
    string CustomerEmail,
    Priority Priority);
