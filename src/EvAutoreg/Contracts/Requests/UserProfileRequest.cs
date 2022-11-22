namespace EvAutoreg.Contracts.Requests;

public readonly record struct UserProfileRequest
{
    public string FisrtName { get; init; }
    public string LastName { get; init; }
}
