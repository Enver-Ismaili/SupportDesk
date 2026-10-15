using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using SupportDesk.Application.Tickets.Validators;

namespace SupportDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateTicketRequestValidator>();
        return services;
    }
}
