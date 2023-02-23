namespace EvAutoreg.Api.Domain;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = default!;
    public bool IsPrivelegedRole { get; set; }
}