namespace EvAutoreg.Contracts.Requests;

public readonly record struct UserEmailRequest
{
    public string NewEmail { get; init; }
}
