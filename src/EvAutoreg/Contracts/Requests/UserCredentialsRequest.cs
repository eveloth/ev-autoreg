namespace EvAutoreg.Contracts.Requests;

public record UserCredentialsRequest(string Email, string Password)
{
    public string Email { get; init; } = Email.ToLower();
}
