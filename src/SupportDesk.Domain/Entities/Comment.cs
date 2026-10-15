namespace SupportDesk.Domain.Entities;

public class Comment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Comment()
    {
    }

    internal Comment(Guid id, Guid ticketId, string authorName, string body, DateTime createdAtUtc)
    {
        Id = id;
        TicketId = ticketId;
        AuthorName = authorName;
        Body = body;
        CreatedAt = createdAtUtc;
    }
}
