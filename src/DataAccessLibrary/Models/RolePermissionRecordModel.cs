namespace DataAccessLibrary.Models;

public class RolePermissionRecordModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public int? PermissionId { get; set; }
    public string? PermissionName { get; set; }
}