using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Agents.Dtos;

public record CreateAgentRequest(
    string FullName,
    string Email,
    Department Department,
    bool IsActive = true);
