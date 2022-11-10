namespace DataAccessLibrary.Models;

public class RolePermissionModel
{
    public RoleModel Role { get; set; }
    public List<PermissionModel> Permissions { get; set; } = new();
}