namespace EvAutoreg.Dto;

public readonly record struct UserPasswordDto
{
    public string NewPassword { get; init; }
}
