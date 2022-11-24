using System.Collections.ObjectModel;

namespace EvAutoreg.Errors;

public static class ErrorCodes
{
    private static readonly Dictionary<int, ApiError> ErrorCodeList =
        new()
        {
            { 1001, new ApiError(1001, "Provided email is invalid.") },
            { 1002, new ApiError(1002, "Provided password is invalid.") },
            { 1003, new ApiError(1003, "Provided email is already taken.") },
            { 1004, new ApiError(1004, "User doesn't exist.") },
            { 1005, new ApiError(1005, "Invalid username and/or password.") },
            { 1006, new ApiError(1006, "User was blocked.") },
            { 2001, new ApiError(2001, "User not found.") },
            { 3001, new ApiError(3001, "Role not found.") },
            { 3002, new ApiError(3002, "User in not assigned to any role.") },
            { 3003, new ApiError(3003, "Role with the specified name already exists.") },
            { 4001, new ApiError(4001, "Permission already exists.") },
            { 4002, new ApiError(4002, "Permission not found.") },
            { 4003, new ApiError(4003, "No such role to permission correlation found.") },
            { 5001, new ApiError(5001, "No credentials assciated with user.") },
            { 9001, new ApiError(9001, "Couldn't perform database transaction.") },
            { 10001, new ApiError(10001, "Service configuration is not valid.") }
        };

    public static ReadOnlyDictionary<int, ApiError> ErrorCode { get; } = new(ErrorCodeList);
}
