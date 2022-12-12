namespace Api.Contracts.Requests;

public record UserEmailRequest(string Email)
{
    public string Email { get; init; } = Email.ToLower();
}