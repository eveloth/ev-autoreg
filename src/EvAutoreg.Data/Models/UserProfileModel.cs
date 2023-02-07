namespace EvAutoreg.Data.Models;

public class UserProfileModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public RoleModel? Role { get; set; } = null;

#pragma warning restore
}