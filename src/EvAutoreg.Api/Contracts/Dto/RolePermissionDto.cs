namespace EvAutoreg.Api.Contracts.Dto;

public class RolePermissionDto
{
    public required RoleDto Role { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}