namespace Api.Contracts.Requests;

public record QueryParametersRequest(
    string WorkTime,
    string RegStatus,
    string? InWorkStatus,
    string? AssignedGroup,
    string? RequestType
);