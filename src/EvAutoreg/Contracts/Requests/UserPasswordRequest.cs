namespace EvAutoreg.Contracts.Requests;

public readonly record struct UserPasswordRequest
{
    public string NewPassword { get; init; }
}
