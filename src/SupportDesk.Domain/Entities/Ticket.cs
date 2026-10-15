using SupportDesk.Domain.Common;
using SupportDesk.Domain.Enums;
using SupportDesk.Domain.Exceptions;
using SupportDesk.Domain.Workflow;

namespace SupportDesk.Domain.Entities;

public class Ticket
{
    private readonly List<Comment> _comments = [];

    public Guid Id { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public Priority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public Guid? AgentId { get; private set; }
    public Agent? Agent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastModifiedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime DueAt { get; private set; }
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private Ticket()
    {
    }

    public static Ticket Create(
        Guid id,
        string reference,
        string title,
        string description,
        string customerName,
        string customerEmail,
        Priority priority,
        DateTime createdAtUtc)
    {
        return new Ticket
        {
            Id = id,
            Reference = reference,
            Title = title,
            Description = description,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            Priority = priority,
            Status = TicketStatus.New,
            CreatedAt = createdAtUtc,
            LastModifiedAt = createdAtUtc,
            DueAt = DueDateCalculator.Calculate(createdAtUtc, priority)
        };
    }

    public bool IsOverdue(DateTime utcNow)
    {
        return utcNow > DueAt
               && Status != TicketStatus.Resolved
               && Status != TicketStatus.Closed;
    }

    public OperationResult UpdateDetails(
        string title,
        string description,
        string customerName,
        string customerEmail,
        Priority priority,
        DateTime utcNow)
    {
        var open = EnsureOpen();
        if (!open.Success)
        {
            return open;
        }

        Title = title;
        Description = description;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        if (Priority != priority)
        {
            Priority = priority;
            DueAt = DueDateCalculator.Calculate(CreatedAt, priority);
        }

        Touch(utcNow);
        return OperationResult.Ok("Ticket updated.");
    }

    public OperationResult Assign(Agent agent, DateTime utcNow)
    {
        var open = EnsureOpen();
        if (!open.Success)
        {
            return open;
        }

        if (!agent.IsActive)
        {
            return OperationResult.Fail(ErrorCodes.InactiveAgent, "An inactive agent cannot be assigned to a ticket.");
        }

        AgentId = agent.Id;
        Agent = agent;
        Touch(utcNow);
        return OperationResult.Ok("Agent assigned.");
    }

    public OperationResult Unassign(DateTime utcNow)
    {
        var open = EnsureOpen();
        if (!open.Success)
        {
            return open;
        }

        if (Status == TicketStatus.InProgress)
        {
            return OperationResult.Fail(
                ErrorCodes.CannotUnassignInProgress,
                "An in-progress ticket must stay assigned to an agent.");
        }

        AgentId = null;
        Agent = null;
        Touch(utcNow);
        return OperationResult.Ok("Agent unassigned.");
    }

    public OperationResult ChangeStatus(TicketStatus target, DateTime utcNow)
    {
        var open = EnsureOpen();
        if (!open.Success)
        {
            return open;
        }

        var transition = TicketStatusMachine.ValidateTransition(Status, target);
        if (!transition.Success)
        {
            return transition;
        }

        if (target == TicketStatus.InProgress)
        {
            if (AgentId is null)
            {
                return OperationResult.Fail(
                    ErrorCodes.AgentRequiredForInProgress,
                    "A ticket cannot move to In Progress unless an active agent is assigned.");
            }

            if (Agent is not null && !Agent.IsActive)
            {
                return OperationResult.Fail(
                    ErrorCodes.InactiveAgent,
                    "A ticket cannot move to In Progress unless an active agent is assigned.");
            }
        }

        Status = target;
        if (target == TicketStatus.Resolved)
        {
            ResolvedAt = utcNow;
            ClosedAt = null;
        }
        else if (target == TicketStatus.Closed)
        {
            ClosedAt = utcNow;
            ResolvedAt ??= utcNow;
        }
        else if (target == TicketStatus.InProgress)
        {
            ResolvedAt = null;
            ClosedAt = null;
        }

        Touch(utcNow);
        return OperationResult.Ok("Status updated.");
    }

    public OperationResult<Comment> AddComment(string authorName, string body, DateTime utcNow)
    {
        var open = EnsureOpen();
        if (!open.Success)
        {
            return OperationResult<Comment>.Fail(open.Code ?? ErrorCodes.TicketClosed, open.Message);
        }

        var comment = new Comment(Guid.NewGuid(), Id, authorName, body, utcNow);
        _comments.Add(comment);
        Touch(utcNow);
        return OperationResult<Comment>.Ok(comment, "Comment added.");
    }

    public IReadOnlyList<TicketStatus> AllowedStatusTargets()
    {
        return Status == TicketStatus.Closed ? [] : TicketStatusMachine.AllowedTargets(Status);
    }

    private OperationResult EnsureOpen()
    {
        if (Status == TicketStatus.Closed)
        {
            return OperationResult.Fail(ErrorCodes.TicketClosed, "A closed ticket is read-only.");
        }

        return OperationResult.Ok();
    }

    private void Touch(DateTime utcNow)
    {
        LastModifiedAt = utcNow;
    }
}
