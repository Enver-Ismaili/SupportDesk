using FluentValidation;
using SupportDesk.Application.Tickets.Dtos;

namespace SupportDesk.Application.Tickets.Validators;

public class ChangeStatusRequestValidator : AbstractValidator<ChangeStatusRequest>
{
    public ChangeStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
