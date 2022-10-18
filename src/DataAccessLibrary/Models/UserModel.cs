namespace DataAccessLibrary.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
}