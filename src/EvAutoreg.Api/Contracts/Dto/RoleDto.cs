namespace EvAutoreg.Api.Contracts.Dto;

public class RoleDto
{
    public required int Id { get; set; }
    public required string RoleName { get; set; }
    public bool IsPrivelegedRole { get; set; }
}