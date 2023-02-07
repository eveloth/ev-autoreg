namespace EvAutoreg.Api.Errors;

public class ApiError
{
    public ApiError(int errorCode, string description)
    {
        ErrorCode = errorCode;
        Description = description;
    }

    public int ErrorCode { get; }
    public string Description { get; }
}