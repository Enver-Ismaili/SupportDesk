using SupportDesk.Application.Common;

namespace SupportDesk.Application.Agents.Dtos;

public class AgentListDto : BooleanDto
{
    public IReadOnlyList<AgentListItemDto> Items { get; set; } = [];
}
