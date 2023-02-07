namespace EvAutoreg.Api.Contracts.Responses;

public record TokenResponse(string JwtToken, string RefreshToken);