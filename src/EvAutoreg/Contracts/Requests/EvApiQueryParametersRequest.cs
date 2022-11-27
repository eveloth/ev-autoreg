namespace EvAutoreg.Contracts.Requests;

public record EvApiQueryParametersRequest(
    int IssueTypeId,
    string WorkTime,
    string RegStatus,
    string? InWorkStatus,
    string? AssignedGroup,
    string? RequestType
);