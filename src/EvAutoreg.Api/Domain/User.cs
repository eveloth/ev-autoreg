namespace EvAutoreg.Api.Domain;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public Role? Role { get; set; }
}