namespace Api.Contracts.Requests;

public record RefreshTokenRequest(string JwtToken, string RefreshToken);