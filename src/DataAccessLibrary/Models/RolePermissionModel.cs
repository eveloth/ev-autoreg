namespace DataAccessLibrary.Models;

public class RolePermissionModel
{
#pragma warning disable CS8618

    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public int? PermissionId { get; set; }
    public string? PermissionName { get; set; }
    public string? Description { get; set; }

#pragma warning restore
}
