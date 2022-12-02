using Autoregistrar.Contracts.Dto;
using DataAccessLibrary.Models;

namespace Autoregistrar.Settings;

public class Settings
{
    public AutoregstrarSettingsModel AutoregSettings { get; set; }
    public ExchangeCredentialsDto ExchangeCredentials { get; set; }
    public EvCredentialsDto ExtraViewCredentials { get; set; }
    public List<EvApiQueryParametersModel> QueryParameters { get; set; } = new();
    public List<RuleModel> Rules { get; set; } = new();
}