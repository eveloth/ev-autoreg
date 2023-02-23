namespace EvAutoreg.Data.Models;

public class PermissionModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string PermissionName { get; set; }
    public string Description { get; set; }
    public bool IsPrivelegedPermission { get; set; }

#pragma warning restore
}