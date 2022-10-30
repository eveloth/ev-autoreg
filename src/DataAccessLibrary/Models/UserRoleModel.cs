namespace DataAccessLibrary.Models;

public class UserRoleModel
{
    #pragma warning disable CS8618
    
    public int UserId { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
    
    #pragma warning restore
}