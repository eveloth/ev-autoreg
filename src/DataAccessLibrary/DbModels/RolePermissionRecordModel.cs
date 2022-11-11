namespace DataAccessLibrary.DbModels;

public class RolePermissionRecordModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public int? PermissionId { get; set; }
    public string? PermissionName { get; set; }
    public string? Description { get; set; }
}