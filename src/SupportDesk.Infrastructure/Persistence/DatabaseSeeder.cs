using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Infrastructure.Persistence;

namespace SupportDesk.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static readonly Guid GraceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid LinusId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid AdaId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid MargaretId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid InactiveId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Agents.AnyAsync(cancellationToken))
        {
            return;
        }

        var grace = new Agent(GraceId, "Grace Hopper", "grace.hopper@supportdesk.local", Department.Technical, true);
        var linus = new Agent(LinusId, "Linus Torvalds", "linus.torvalds@supportdesk.local", Department.Technical, true);
        var ada = new Agent(AdaId, "Ada Lovelace", "ada.lovelace@supportdesk.local", Department.Billing, true);
        var margaret = new Agent(MargaretId, "Margaret Hamilton", "margaret.hamilton@supportdesk.local", Department.General, true);
        var retired = new Agent(InactiveId, "Retired Agent", "retired.agent@supportdesk.local", Department.General, false);

        db.Agents.AddRange(grace, linus, ada, margaret, retired);

        var now = DateTime.UtcNow;
        var tickets = new List<Ticket>();

        tickets.Add(CreateOpen("0001", "Cannot reset password", "Reset emails never arrive.", "Nora Allen", "nora@example.com", Priority.High, now.AddDays(-2), grace, TicketStatus.InProgress, now.AddDays(-1)));
        tickets.Add(CreateOpen("0002", "Invoice PDF is blank", "March invoice downloads as an empty file.", "Barry Allen", "barry@example.com", Priority.Normal, now.AddDays(-1), ada, TicketStatus.InProgress, now.AddHours(-8)));
        tickets.Add(CreateOpen("0003", "SSO login loop", "Redirects back to the identity provider forever.", "Iris West", "iris@example.com", Priority.Critical, now.AddHours(-10), linus, TicketStatus.InProgress, now.AddHours(-9)));
        tickets.Add(CreateOpen("0004", "Wrong currency on quote", "Quote shows USD instead of EUR.", "Cisco Ramon", "cisco@example.com", Priority.Low, now.AddDays(-4), ada, TicketStatus.New));
        tickets.Add(CreateOpen("0005", "Mobile app crash on launch", "iOS 18 crash after splash screen.", "Caitlin Snow", "caitlin@example.com", Priority.High, now.AddHours(-6), margaret, TicketStatus.New));
        tickets.Add(CreateResolved("0006", "API rate limit confusion", "429 responses on the public API.", "Harrison Wells", "wells@example.com", Priority.Normal, now.AddDays(-5), linus, now.AddDays(-1)));
        tickets.Add(CreateClosed("0007", "Typo on billing portal", "Button labelled Recieve.", "Joe West", "joe@example.com", Priority.Low, now.AddDays(-12), ada, now.AddDays(-3), now.AddDays(-2)));
        tickets.Add(CreateOpen("0008", "Overdue critical outage", "Production dashboard is down.", "Kara Danvers", "kara@example.com", Priority.Critical, now.AddDays(-3), grace, TicketStatus.InProgress, now.AddDays(-3).AddHours(1)));
        tickets.Add(CreateOpen("0009", "Overdue high priority access", "Cannot add a teammate.", "Clark Kent", "clark@example.com", Priority.High, now.AddDays(-4), null, TicketStatus.New));
        tickets.Add(CreateOpen("0010", "Need export of last year tickets", "CSV export for audit.", "Lois Lane", "lois@example.com", Priority.Normal, now.AddDays(-1), margaret, TicketStatus.New));
        tickets.Add(CreateOpen("0011", "Two-factor codes delayed", "SMS arrives after 10 minutes.", "Bruce Wayne", "bruce@example.com", Priority.High, now.AddHours(-20), grace, TicketStatus.InProgress, now.AddHours(-18)));
        tickets.Add(CreateResolved("0012", "Duplicate customer records", "Same email created twice.", "Diana Prince", "diana@example.com", Priority.Normal, now.AddDays(-8), ada, now.AddHours(-6)));
        tickets.Add(CreateClosed("0013", "Dark mode contrast", "Low contrast on comments.", "Selina Kyle", "selina@example.com", Priority.Low, now.AddDays(-20), margaret, now.AddDays(-6), now.AddDays(-5)));
        tickets.Add(CreateOpen("0014", "Webhook retries never stop", "Partner receiving 50 retries.", "Oliver Queen", "oliver@example.com", Priority.Critical, now.AddHours(-2), linus, TicketStatus.New));
        tickets.Add(CreateOpen("0015", "Cannot change department", "Profile save fails silently.", "Felicity Smoak", "felicity@example.com", Priority.Normal, now.AddDays(-2), null, TicketStatus.New));
        tickets.Add(CreateOpen("0016", "Overdue billing dispute", "Customer billed after cancellation.", "John Diggle", "diggle@example.com", Priority.High, now.AddDays(-6), ada, TicketStatus.InProgress, now.AddDays(-5)));
        tickets.Add(CreateOpen("0017", "Search ignores ticket reference", "TCK search returns nothing.", "Dinah Lance", "dinah@example.com", Priority.Low, now.AddDays(-3), margaret, TicketStatus.InProgress, now.AddDays(-2)));
        tickets.Add(CreateResolved("0018", "Email footer still old brand", "Old logo in notifications.", "Roy Harper", "roy@example.com", Priority.Low, now.AddDays(-9), grace, now.AddDays(-1)));
        tickets.Add(CreateClosed("0019", "Onboarding checklist missing step", "Agents skip assignment.", "Thea Queen", "thea@example.com", Priority.Normal, now.AddDays(-15), linus, now.AddDays(-4), now.AddDays(-3)));
        tickets.Add(CreateOpen("0020", "Need Portuguese language pack", "Support portal English only.", "Rene Ramirez", "rene@example.com", Priority.Low, now.AddHours(-12), null, TicketStatus.New));

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static Ticket CreateOpen(
        string seq,
        string title,
        string description,
        string customer,
        string email,
        Priority priority,
        DateTime createdAt,
        Agent? agent,
        TicketStatus status,
        DateTime? assignedAt = null)
    {
        var ticket = Ticket.Create(Guid.NewGuid(), $"TCK-{createdAt.Year}-{seq}", title, description, customer, email, priority, createdAt);
        if (agent is not null)
        {
            ticket.Assign(agent, assignedAt ?? createdAt);
        }

        if (status == TicketStatus.InProgress)
        {
            ticket.ChangeStatus(TicketStatus.InProgress, assignedAt ?? createdAt.AddMinutes(30));
        }

        ticket.AddComment(agent?.FullName ?? customer, "Initial report captured from the intake form.", createdAt.AddMinutes(5));
        return ticket;
    }

    private static Ticket CreateResolved(
        string seq,
        string title,
        string description,
        string customer,
        string email,
        Priority priority,
        DateTime createdAt,
        Agent agent,
        DateTime resolvedAt)
    {
        var ticket = CreateOpen(seq, title, description, customer, email, priority, createdAt, agent, TicketStatus.InProgress, createdAt.AddHours(1));
        ticket.ChangeStatus(TicketStatus.Resolved, resolvedAt);
        ticket.AddComment(agent.FullName, "Root cause addressed. Waiting for customer confirmation.", resolvedAt);
        return ticket;
    }

    private static Ticket CreateClosed(
        string seq,
        string title,
        string description,
        string customer,
        string email,
        Priority priority,
        DateTime createdAt,
        Agent agent,
        DateTime resolvedAt,
        DateTime closedAt)
    {
        var ticket = CreateResolved(seq, title, description, customer, email, priority, createdAt, agent, resolvedAt);
        ticket.ChangeStatus(TicketStatus.Closed, closedAt);
        return ticket;
    }
}
