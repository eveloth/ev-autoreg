namespace EvAutoreg.Api.Domain;

public class RolePermission
{
#pragma warning disable CS8618
    public Role Role { get; set; }
    public List<Permission> Permissions { get; set; } = new();
#pragma warning restore CS8618
}