using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Agents.Dtos;

public record UpdateAgentRequest(
    string FullName,
    string Email,
    Department Department);
