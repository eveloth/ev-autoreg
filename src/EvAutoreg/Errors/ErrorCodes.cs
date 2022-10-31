using System.Collections.ObjectModel;

namespace EvAutoreg.Errors;
public static class ErrorCodes
{
    private static readonly Dictionary<int, ApiError> _errorCode = new Dictionary<int, ApiError>()
    {
        { 1001, new ApiError(1001, "Provided email is invalid.") },
        { 1002, new ApiError(1002, "Provided password is invalid.") },
        { 1003, new ApiError(1003, "Provided email is already taken.") },
        { 1004, new ApiError(1004, "User doesn't exist.") },
        { 1005, new ApiError(1005, "Invalid username and/or password.") },
        { 1006, new ApiError(1006, "User was blocked.") },
        { 2001, new ApiError(2001, "User not found.") },
        { 3001, new ApiError(3001, "Role not found.") },
        { 3002, new ApiError(3002, "User role correlation doesn't exist.") },
        { 9001, new ApiError(9001, "Couldn't perform database transaction.") }
    };

    public static ReadOnlyDictionary<int, ApiError> ErrorCode { get; } = new(_errorCode);
}
