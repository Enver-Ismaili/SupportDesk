using SupportDesk.Application.Agents.Dtos;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Agents;

public static class AgentMapper
{
    public static AgentListItemDto ToListItem(Agent agent)
    {
        return new AgentListItemDto
        {
            Id = agent.Id,
            FullName = agent.FullName,
            Email = agent.Email,
            Department = agent.Department,
            IsActive = agent.IsActive
        };
    }

    public static AgentDto ToDto(Agent agent, string message = "OK")
    {
        return new AgentDto
        {
            Success = true,
            Message = message,
            Id = agent.Id,
            FullName = agent.FullName,
            Email = agent.Email,
            Department = agent.Department,
            IsActive = agent.IsActive
        };
    }

    public static AgentDto Fail(string message)
    {
        return new AgentDto
        {
            Success = false,
            Message = message
        };
    }

    public static AgentListDto FailList(string message)
    {
        return new AgentListDto
        {
            Success = false,
            Message = message
        };
    }
}
