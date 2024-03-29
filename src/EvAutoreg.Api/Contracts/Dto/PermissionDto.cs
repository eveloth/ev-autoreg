namespace EvAutoreg.Api.Contracts.Dto;

public class PermissionDto
{
    public required int Id { get; set; }
    public required string PermissionName { get; set; }
    public required string Description { get; set; }
    public bool IsPrivelegedPermission { get; set; }
}