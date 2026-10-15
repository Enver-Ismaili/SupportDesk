using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Domain.Exceptions;
using SupportDesk.Domain.Workflow;

namespace SupportDesk.Domain.Tests;

public class TicketStatusMachineTests
{
    private static readonly DateTime Now = new(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Agent ActiveAgent() =>
        new(Guid.NewGuid(), "Grace Hopper", "grace@example.com", Department.Technical, true);

    private static Agent InactiveAgent() =>
        new(Guid.NewGuid(), "Inactive", "off@example.com", Department.General, false);

    private static Ticket NewTicket() =>
        Ticket.Create(
            Guid.NewGuid(),
            "TCK-2026-0002",
            "Billing error",
            "Charged twice",
            "Alan Turing",
            "alan@example.com",
            Priority.High,
            Now);

    [Fact]
    public void Allowed_transitions_succeed()
    {
        var ticket = NewTicket();
        var agent = ActiveAgent();
        Assert.True(ticket.Assign(agent, Now).Success);

        Assert.True(ticket.ChangeStatus(TicketStatus.InProgress, Now).Success);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);

        Assert.True(ticket.ChangeStatus(TicketStatus.Resolved, Now.AddHours(1)).Success);
        Assert.Equal(TicketStatus.Resolved, ticket.Status);
        Assert.Equal(Now.AddHours(1), ticket.ResolvedAt);

        Assert.True(ticket.ChangeStatus(TicketStatus.InProgress, Now.AddHours(2)).Success);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
        Assert.Null(ticket.ResolvedAt);

        Assert.True(ticket.ChangeStatus(TicketStatus.Resolved, Now.AddHours(3)).Success);
        Assert.True(ticket.ChangeStatus(TicketStatus.Closed, Now.AddHours(4)).Success);
        Assert.Equal(TicketStatus.Closed, ticket.Status);
        Assert.Equal(Now.AddHours(4), ticket.ClosedAt);
    }

    [Fact]
    public void New_to_resolved_is_rejected()
    {
        var ticket = NewTicket();
        var result = ticket.ChangeStatus(TicketStatus.Resolved, Now);
        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.StatusTransitionNotAllowed, result.Code);
    }

    [Fact]
    public void Closed_ticket_cannot_be_reopened()
    {
        var ticket = NewTicket();
        ticket.Assign(ActiveAgent(), Now);
        ticket.ChangeStatus(TicketStatus.InProgress, Now);
        ticket.ChangeStatus(TicketStatus.Resolved, Now);
        ticket.ChangeStatus(TicketStatus.Closed, Now);

        var result = ticket.ChangeStatus(TicketStatus.InProgress, Now);
        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.TicketClosed, result.Code);
    }

    [Fact]
    public void In_progress_without_agent_is_rejected()
    {
        var ticket = NewTicket();
        var result = ticket.ChangeStatus(TicketStatus.InProgress, Now);
        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.AgentRequiredForInProgress, result.Code);
    }

    [Fact]
    public void Inactive_agent_cannot_be_assigned()
    {
        var ticket = NewTicket();
        var result = ticket.Assign(InactiveAgent(), Now);
        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.InactiveAgent, result.Code);
    }

    [Fact]
    public void Closed_ticket_rejects_comments_and_edits()
    {
        var ticket = NewTicket();
        ticket.Assign(ActiveAgent(), Now);
        ticket.ChangeStatus(TicketStatus.InProgress, Now);
        ticket.ChangeStatus(TicketStatus.Resolved, Now);
        ticket.ChangeStatus(TicketStatus.Closed, Now);

        var comment = ticket.AddComment("Ada", "Hello", Now);
        Assert.False(comment.Success);
        Assert.Equal(ErrorCodes.TicketClosed, comment.Code);

        var edit = ticket.UpdateDetails("x", "y", "z", "z@example.com", Priority.Low, Now);
        Assert.False(edit.Success);
        Assert.Equal(ErrorCodes.TicketClosed, edit.Code);
    }

    [Fact]
    public void Machine_rejects_in_progress_to_closed()
    {
        Assert.False(TicketStatusMachine.CanTransition(TicketStatus.InProgress, TicketStatus.Closed));
        var result = TicketStatusMachine.ValidateTransition(TicketStatus.InProgress, TicketStatus.Closed);
        Assert.False(result.Success);
        Assert.Equal(ErrorCodes.StatusTransitionNotAllowed, result.Code);
    }
}
