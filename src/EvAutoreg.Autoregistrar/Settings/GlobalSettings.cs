using EvAutoreg.Autoregistrar.Domain;

namespace EvAutoreg.Autoregistrar.Settings;

public static class GlobalSettings
{
    public static AutoregistrarSettings? AutoregistrarSettings { get; set; }
    public static ExchangeCredentials? ExchangeCredentials { get; set; }
    public static ExtraViewCredentials? ExtraViewCredentials { get; set; }
    public static List<IssueField>? IssueFields { get; set; } = new();
    public static List<IssueTypeInfo>? IssueTypes { get; set; } = new();
}