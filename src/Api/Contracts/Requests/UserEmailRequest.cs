namespace Api.Contracts.Requests;

public record UserEmailRequest(string NewEmail)
{
    public string NewEmail { get; init; } = NewEmail.ToLower();
}
