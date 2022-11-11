namespace DataAccessLibrary.DisplayModels;

public class UserRoleDisplayModel
{
#pragma warning disable CS8618

    public int UserId { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }

#pragma warning restore
}
