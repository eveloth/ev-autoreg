namespace EvAutoreg.Contracts.Requests;

public record EvCredentialsRequest(string Email, string Password)
{
    public string Email { get; init; } = Email.ToLower();
}