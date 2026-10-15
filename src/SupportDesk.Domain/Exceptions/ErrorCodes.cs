namespace SupportDesk.Domain.Exceptions;

public static class ErrorCodes
{
    public const string TicketClosed = "TICKET_CLOSED";
    public const string StatusTransitionNotAllowed = "STATUS_TRANSITION_NOT_ALLOWED";
    public const string AgentRequiredForInProgress = "AGENT_REQUIRED_FOR_IN_PROGRESS";
    public const string InactiveAgent = "INACTIVE_AGENT";
    public const string CannotUnassignInProgress = "CANNOT_UNASSIGN_IN_PROGRESS";
    public const string AgentNotFound = "AGENT_NOT_FOUND";
    public const string TicketNotFound = "TICKET_NOT_FOUND";
    public const string AgentEmailTaken = "AGENT_EMAIL_TAKEN";
    public const string AgentHasTickets = "AGENT_HAS_TICKETS";
}
