namespace EvAutoreg.Contracts.Requests;

public readonly record struct UserCredentialsRequest
{
    public string Email { get; init; }
    public string Password { get; init; }
}
