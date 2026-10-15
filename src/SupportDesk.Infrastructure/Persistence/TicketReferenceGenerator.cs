using Microsoft.EntityFrameworkCore;

namespace SupportDesk.Infrastructure.Persistence;

public class TicketReferenceGenerator
{
    public async Task<string> NextAsync(AppDbContext db, DateTime utcNow, CancellationToken cancellationToken)
    {
        var year = utcNow.Year;
        var prefix = $"TCK-{year}-";

        var last = await db.Tickets
            .Where(t => t.Reference.StartsWith(prefix))
            .OrderByDescending(t => t.Reference)
            .Select(t => t.Reference)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = 1;
        if (last is not null && last.Length > prefix.Length && int.TryParse(last[prefix.Length..], out var parsed))
        {
            nextNumber = parsed + 1;
        }

        return $"{prefix}{nextNumber:0000}";
    }
}
