using Api.Errors;

namespace Api.Contracts.Responses;

public record ErrorResponse
{
    public required ApiError ApiError { get; init; }
    public List<string> Details { get; init; } = new();
}