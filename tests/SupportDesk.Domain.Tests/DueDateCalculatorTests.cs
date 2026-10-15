using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Domain.Workflow;

namespace SupportDesk.Domain.Tests;

public class DueDateCalculatorTests
{
    private static readonly DateTime CreatedAt = new(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(Priority.Critical, 4)]
    public void Critical_is_four_hours_after_creation(Priority priority, int hours)
    {
        var due = DueDateCalculator.Calculate(CreatedAt, priority);
        Assert.Equal(CreatedAt.AddHours(hours), due);
    }

    [Theory]
    [InlineData(Priority.High, 1)]
    [InlineData(Priority.Normal, 3)]
    [InlineData(Priority.Low, 7)]
    public void Non_critical_offsets_are_in_days(Priority priority, int days)
    {
        var due = DueDateCalculator.Calculate(CreatedAt, priority);
        Assert.Equal(CreatedAt.AddDays(days), due);
    }

    [Fact]
    public void Changing_priority_recalculates_from_original_created_date()
    {
        var ticket = Ticket.Create(
            Guid.NewGuid(),
            "TCK-2026-0001",
            "Login issue",
            "Cannot sign in",
            "Ada Lovelace",
            "ada@example.com",
            Priority.Normal,
            CreatedAt);

        Assert.Equal(CreatedAt.AddDays(3), ticket.DueAt);

        var update = ticket.UpdateDetails(
            ticket.Title,
            ticket.Description,
            ticket.CustomerName,
            ticket.CustomerEmail,
            Priority.Critical,
            CreatedAt.AddHours(2));

        Assert.True(update.Success);
        Assert.Equal(CreatedAt.AddHours(4), ticket.DueAt);
        Assert.Equal(CreatedAt, ticket.CreatedAt);
    }
}
