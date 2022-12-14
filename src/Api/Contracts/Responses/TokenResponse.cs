namespace Api.Contracts.Responses;

public record TokenResponse(string JwtToken, string RefreshToken);