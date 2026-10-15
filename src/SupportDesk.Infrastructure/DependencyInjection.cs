using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Agents;
using SupportDesk.Application.Tickets;
using SupportDesk.Infrastructure.Persistence;
using SupportDesk.Infrastructure.Services;

namespace SupportDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<TicketReferenceGenerator>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IAgentService, AgentService>();
        return services;
    }
}
