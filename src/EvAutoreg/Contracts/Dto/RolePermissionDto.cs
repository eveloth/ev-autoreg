namespace EvAutoreg.Contracts.Dto;

public class RolePermissionDto
{
#pragma warning disable CS8618
    
    public RoleDto Role { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}