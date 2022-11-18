namespace DataAccessLibrary.SqlDataAccess;

public static class SqlDataAccessOptions
{
    public static bool HasAffix { get; set; }
    public static string Affix { get; set; } = string.Empty;
    public static string SplitOn { get; set; } = "Id";
}
