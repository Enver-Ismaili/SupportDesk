using SupportDesk.Domain.Common;
using SupportDesk.Domain.Enums;
using SupportDesk.Domain.Exceptions;

namespace SupportDesk.Domain.Workflow;

public static class TicketStatusMachine
{
    public static bool CanTransition(TicketStatus from, TicketStatus to)
    {
        return (from, to) switch
        {
            (TicketStatus.New, TicketStatus.InProgress) => true,
            (TicketStatus.InProgress, TicketStatus.Resolved) => true,
            (TicketStatus.Resolved, TicketStatus.Closed) => true,
            (TicketStatus.Resolved, TicketStatus.InProgress) => true,
            _ => false
        };
    }

    public static IReadOnlyList<TicketStatus> AllowedTargets(TicketStatus from)
    {
        return from switch
        {
            TicketStatus.New => [TicketStatus.InProgress],
            TicketStatus.InProgress => [TicketStatus.Resolved],
            TicketStatus.Resolved => [TicketStatus.Closed, TicketStatus.InProgress],
            _ => []
        };
    }

    public static OperationResult ValidateTransition(TicketStatus from, TicketStatus to)
    {
        if (from == to)
        {
            return OperationResult.Fail(
                ErrorCodes.StatusTransitionNotAllowed,
                $"Ticket is already {from}.");
        }

        if (from == TicketStatus.Closed)
        {
            return OperationResult.Fail(
                ErrorCodes.TicketClosed,
                "A closed ticket cannot change status.");
        }

        if (!CanTransition(from, to))
        {
            return OperationResult.Fail(
                ErrorCodes.StatusTransitionNotAllowed,
                $"Cannot change status from {from} to {to}. Allowed transitions: New to In Progress, In Progress to Resolved, Resolved to Closed, and Resolved to In Progress.");
        }

        return OperationResult.Ok();
    }
}
