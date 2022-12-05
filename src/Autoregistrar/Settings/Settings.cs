using Autoregistrar.Domain;

namespace Autoregistrar.Settings;

public class Settings
{
    public required AutoregistrarSettings AutoregistrarSettings { get; set; }
    public required ExchangeCredentials ExchangeCredentials { get; set; }
    public required ExtraViewCredentials ExtraViewCredentials { get; set; }
    public required List<IssueField> IssueFields { get; set; }
    public required List<IssueType> IssueTypes { get; set; }
}