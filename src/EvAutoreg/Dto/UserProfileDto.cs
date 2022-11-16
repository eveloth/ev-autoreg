namespace EvAutoreg.Dto;

public readonly record struct UserProfileDto
{
    public string FisrtName { get; init; }
    public string LastName { get; init; }
}
