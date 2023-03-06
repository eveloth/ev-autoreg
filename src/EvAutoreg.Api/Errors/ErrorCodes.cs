using System.Collections.ObjectModel;

namespace EvAutoreg.Api.Errors;

public static class ErrorCodes
{
    private static readonly Dictionary<int, ApiError> ErrorCodeList =
        new()
        {
            { 1001, new ApiError(1001, "Provided email is already taken.") },
            { 1002, new ApiError(1002, "Password can not be the same as the email.") },
            { 1003, new ApiError(1003, "User was blocked.") },
            { 1004, new ApiError(1004, "User not found") },
            { 1005, new ApiError(1005, "Authentication failed.") },
            { 1006, new ApiError(1006, "Token is invalid.") },
            {
                1007,
                new ApiError(
                    1007,
                    "User in priveleged role cannot be deleted, remove them from role first."
                )
            },
            {
                1008,
                new ApiError(
                    1008,
                    "User in priveleged role cannot be blocked, remove them from role first."
                )
            },
            { 1013, new ApiError(1013, "You cannot block or delete your own account.") },
            { 2001, new ApiError(2001, "Role name is already taken.") },
            { 2002, new ApiError(2002, "User is not assigned to any role.") },
            { 2003, new ApiError(2003, "Role is priveleged, insufficient rights.") },
            { 2004, new ApiError(2004, "Role not found.") },
            {
                2005,
                new ApiError(
                    2005,
                    "Priveleged role cannot be deleted, remove priveleged permissions first."
                )
            },
            { 2006, new ApiError(2006, "You cannot remove yourself from priveleged role.") },
            { 3001, new ApiError(3001, "Permission already exists.") },
            { 3002, new ApiError(3002, "Permission is priveleged, insufficient rights.") },
            { 3004, new ApiError(3004, "Permission not found.") },
            { 4001, new ApiError(4001, "Correlation already exists.") },
            { 4004, new ApiError(4004, "Correlation not found.") },
            { 5004, new ApiError(5004, "Issue not found") },
            { 6004, new ApiError(6004, "Rule not found.") },
            { 6014, new ApiError(6014, "Rule set not found.") },
            { 7001, new ApiError(7001, "Issue type with the specified name already exists.") },
            { 7004, new ApiError(7004, "Issue type not found.") },
            { 8004, new ApiError(8004, "Issue field not found.") },
            { 9004, new ApiError(9004, "Autoregistrar settings not found.") },
            { 10001, new ApiError(10001, "ExtraView API query parameters already exist.") },
            {
                10002,
                new ApiError(
                    10002,
                    "Query parameters execution order must differ from already specified."
                )
            },
            { 10004, new ApiError(10004, "ExtraView API query parameters not found.") },
            { 11001, new ApiError(11001, "Couldn't start autoregistar; it is not stopped.") },
            { 11002, new ApiError(11002, "Couldn't stop autoregistar; it is not started.") },
            {
                11003,
                new ApiError(11003, "Only the owner of the current session can terminate it.")
            },
            { 12001, new ApiError(12001, "Validation error") },
            { 13001, new ApiError(13001, "Internal error. Consult the service administrator.") },
            {
                13002,
                new ApiError(
                    13002,
                    "Exchange credentials are invalid or Exchange server is inreachable."
                )
            },
            { 14001, new ApiError(14001, "Configuration file is not valid, check entries.") },
        };

    public static ReadOnlyDictionary<int, ApiError> ErrorCode { get; } = new(ErrorCodeList);
}