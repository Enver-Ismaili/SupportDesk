using SupportDesk.Domain.Enums;

namespace SupportDesk.Domain.Entities;

public class Agent
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Department Department { get; private set; }
    public bool IsActive { get; private set; }

    private Agent()
    {
    }

    public Agent(Guid id, string fullName, string email, Department department, bool isActive)
    {
        Id = id;
        FullName = fullName.Trim();
        Email = email.Trim();
        Department = department;
        IsActive = isActive;
    }

    public void Update(string fullName, string email, Department department)
    {
        FullName = fullName.Trim();
        Email = email.Trim();
        Department = department;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
