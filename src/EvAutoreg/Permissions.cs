namespace EvAutoreg;

public static class Permissions
{
    public enum UserPermission
    {
        ReadUsers,
        ResetUserPassword,
        BlockUsers,
        DeleteUsers
    }

    public enum UserRolePermission
    {
        ReadRole,
        CreateRole,
        UpdateRole,
        DeleteRole,
        ReadUserRole,
        AddUserToRole,
        UpdateUserRole,
        RemoveUserFromRole
    }

    public enum ForecastRole
    {
        ReadForecast
    }

    public static IEnumerable<string> GetPermissions<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetNames<TEnum>();
    }
}