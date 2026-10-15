using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Tickets.Dtos;

public record ChangeStatusRequest(TicketStatus Status);
