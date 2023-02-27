namespace EvAutoreg.Api.Contracts.Dto;

public class RolePermissionDto
{

    public required int RoleId { get; set; }
    public required string RoleName { get; set; }
    public bool IsPrivelegedRole { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}