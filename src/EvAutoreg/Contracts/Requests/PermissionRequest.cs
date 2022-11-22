namespace EvAutoreg.Contracts.Requests;

public readonly record struct PermissionRequest
{
    public string PermissionName { get; init; }
    public string Description { get; init; }
}
