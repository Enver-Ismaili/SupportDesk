using SupportDesk.Application.Agents.Dtos;
using SupportDesk.Application.Common;

namespace SupportDesk.Application.Agents;

public interface IAgentService
{
    Task<AgentListDto> ListAsync(string? search, CancellationToken cancellationToken);
    Task<AgentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<AgentDto> CreateAsync(CreateAgentRequest request, CancellationToken cancellationToken);
    Task<AgentDto> UpdateAsync(Guid id, UpdateAgentRequest request, CancellationToken cancellationToken);
    Task<BooleanDto> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<AgentDto> SetActiveAsync(Guid id, SetAgentActiveRequest request, CancellationToken cancellationToken);
}
