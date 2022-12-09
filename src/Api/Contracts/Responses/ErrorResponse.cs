using Api.Errors;

namespace Api.Contracts.Responses;

public class ErrorResponse
{
    public ApiError ApiError { get; set; }
    public List<string> Details { get; set; } = new();
}