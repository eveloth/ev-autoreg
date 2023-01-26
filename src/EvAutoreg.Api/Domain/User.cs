namespace EvAutoreg.Api.Domain;

public class User
{
#pragma warning disable CS8618
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public Role? Role { get; set; }
#pragma warning restore CS8618
}