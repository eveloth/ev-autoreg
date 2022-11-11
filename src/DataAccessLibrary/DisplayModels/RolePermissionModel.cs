namespace DataAccessLibrary.DisplayModels;

public class RolePermissionModel
{
    public RoleModel Role { get; set; }
    public List<PermissionModel> Permissions { get; set; } = new();
}