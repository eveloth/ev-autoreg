namespace EvAutoreg.Api.Domain;

public class Permission
{
    public int Id { get; set; }
    public  string PermissionName { get; set; } = default!;
    public  string Description { get; set; } = default!;
    public bool IsPrivelegedPermission { get; set; }
}