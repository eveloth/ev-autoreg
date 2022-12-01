namespace Api.Contracts.Requests;

public record RoleRequest(string RoleName)
{
    public string RoleName { get; init; } = RoleName.ToLower();
}