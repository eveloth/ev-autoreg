namespace EvAutoreg.Api.Domain;

public class RolePermission
{
    public Role Role { get; set; } = default!;
    public List<Permission> Permissions { get; set; } = new();
}