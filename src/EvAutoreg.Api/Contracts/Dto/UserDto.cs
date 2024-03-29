﻿namespace EvAutoreg.Api.Contracts.Dto;

public class UserDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required bool IsBlocked { get; set; }
    public required bool IsDeleted { get; set; }
    public RoleDto? Role { get; set; } = null;
}