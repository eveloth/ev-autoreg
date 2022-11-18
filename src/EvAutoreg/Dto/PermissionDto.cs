namespace EvAutoreg.Dto;

public readonly record struct PermissionDto
{
    public string PermissionName { get; init; }
    public string Description { get; init; }
}
