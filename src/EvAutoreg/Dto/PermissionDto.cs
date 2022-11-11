namespace EvAutoreg.Dto;

public record struct PermissionDto
{
    public string PermissionName { get; set; }
    public string Description { get; set; }
}