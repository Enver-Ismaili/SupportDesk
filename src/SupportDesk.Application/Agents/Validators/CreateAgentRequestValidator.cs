using FluentValidation;
using SupportDesk.Application.Agents.Dtos;

namespace SupportDesk.Application.Agents.Validators;

public class CreateAgentRequestValidator : AbstractValidator<CreateAgentRequest>
{
    public CreateAgentRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Department).IsInEnum();
    }
}
