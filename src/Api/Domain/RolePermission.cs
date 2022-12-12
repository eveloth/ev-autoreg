namespace Api.Domain;

public class RolePermission
{
    public Role Role { get; set; }
    public List<Permission> Permissions { get; set; } = new();
}