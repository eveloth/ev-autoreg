namespace EvAutoreg.Contracts.Requests;

public record EvApiQueryParametersRequest(
    string WorkTime,
    string RegStatus,
    string? InWorkStatus,
    string? AssignedGroup,
    string? RequestType
);