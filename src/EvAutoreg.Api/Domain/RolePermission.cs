namespace EvAutoreg.Api.Domain;

public class RolePermission
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public bool IsPrivelegedRole { get; set; }
    public List<Permission> Permissions { get; set; } = new();
}