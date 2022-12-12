namespace Api.Domain;

public class Permission
{
    public int Id { get; set; }
    public required string PermissionName { get; set; }
    public required string Description { get; set; }
}