using SupportDesk.Domain.Enums;

namespace SupportDesk.Domain.Workflow;

public static class DueDateCalculator
{
    public static DateTime Calculate(DateTime createdAtUtc, Priority priority)
    {
        return priority switch
        {
            Priority.Critical => createdAtUtc.AddHours(4),
            Priority.High => createdAtUtc.AddDays(1),
            Priority.Normal => createdAtUtc.AddDays(3),
            Priority.Low => createdAtUtc.AddDays(7),
            _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Unknown priority.")
        };
    }
}
