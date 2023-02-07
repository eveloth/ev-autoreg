namespace EvAutoreg.Api.Contracts.Requests;

public record UserCredentialsRequest(string Email, string Password)
{
    public string Email { get; init; } = Email.ToLower();
    public string Password { get; init; } = Password;
}