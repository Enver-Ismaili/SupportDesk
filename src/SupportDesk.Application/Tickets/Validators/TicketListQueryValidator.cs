using FluentValidation;
using SupportDesk.Application.Tickets.Dtos;

namespace SupportDesk.Application.Tickets.Validators;

public class TicketListQueryValidator : AbstractValidator<TicketListQuery>
{
    public TicketListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.Priority).IsInEnum().When(x => x.Priority.HasValue);
    }
}
