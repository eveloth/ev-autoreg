namespace EvAutoreg.Dto;

public readonly record struct UserCredentialsDto
{
    public string Email { get; init; }
    public string Password { get; init; }
}
