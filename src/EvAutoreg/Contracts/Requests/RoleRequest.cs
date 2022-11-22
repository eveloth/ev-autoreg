namespace EvAutoreg.Contracts.Requests;

public readonly record struct RoleRequest
{
    public string RoleName { get; init; }
}
