namespace EvAutoreg.Api.Contracts.Requests;

public record QueryParametersRequest(
    string WorkTime,
    string Status,
    string? AssignedGroup,
    string? RequestType,
    int ExecutionOrder
);