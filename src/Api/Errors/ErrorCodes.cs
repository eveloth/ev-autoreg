using System.Collections.ObjectModel;

namespace Api.Errors;

public static class ErrorCodes
{
    private static readonly Dictionary<int, ApiError> ErrorCodeList =
        new()
        {
            { 1001, new ApiError(1001, "Provided email is already taken.") },
            { 1003, new ApiError(1003, "User was blocked.") },
            { 1004, new ApiError(1004, "User not found") },
            { 2001, new ApiError(2001, "Role name is already taken.") },
            { 2002, new ApiError(2002, "User in not assigned to any role.") },
            { 2004, new ApiError(2004, "Role not found.") },
            { 3001, new ApiError(3001, "Permission already exists.") },
            { 3004, new ApiError(3004, "Permission not found.") },
            { 4004, new ApiError(4004, "Correlation not found.") },
            { 5004, new ApiError(5004, "No external credentials associated with user.") },
            { 6004, new ApiError(6004, "Rule doesn't exist.") },
            { 7001, new ApiError(7001, "Issue type with the specified name already exists.") },
            { 7003, new ApiError(7004, "Issue type doesn't exist.") },
            { 8004, new ApiError(8004, "Issue field doesn't exist.") },
            { 9004, new ApiError(9004, "Autoregistrar settings not found.") },
            { 10004, new ApiError(10004, "ExtraView API query parameters not found.") },
            { 11001, new ApiError(11001, "Couldn't start autoregistar; it is not stopped.") },
            { 11002, new ApiError(11002, "Couldn't stop autoregistar; it is not started.") },
            { 11003, new ApiError(11003, "Only the owner of the current session can terminate it.") },
            { 12001, new ApiError(12001, "Validation error") },
            { 13001, new ApiError(13001, "Couldn't perform database transaction.") },
            { 14001, new ApiError(14001, "Configuration file is not valid, check entries.") },
        };

    public static ReadOnlyDictionary<int, ApiError> ErrorCode { get; } = new(ErrorCodeList);
}