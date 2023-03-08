using System.Text.RegularExpressions;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Data.Models;
using Mapster;

namespace EvAutoreg.Autoregistrar.Mapping;

public static class MappingConfig
{
    public static void ConfigureDbToDomainMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<AutoregstrarSettingsModel, AutoregistrarSettings>
            .NewConfig()
            .Map(dest => dest.ExchangeServerUri, src => src.ExchangeServerUri)
            .Map(dest => dest.ExtraViewUri, src => src.ExtraViewUri)
            .Map(dest => dest.NewIssueRegex, src => new Regex(src.NewIssueRegex))
            .Map(dest => dest.IssueNoRegex, src => new Regex(src.IssueNoRegex))
            .IgnoreNullValues(true);

        TypeAdapterConfig<
            (IssueTypeModel, IEnumerable<QueryParametersModel>, IEnumerable<FilledRuleSetModel>),
            IssueTypeInfo
        >
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.IssueTypeName, src => src.Item1.IssueTypeName)
            .Map(dest => dest.QueryParameters, src => src.Item2)
            .Map(dest => dest.RuleSets, src => src.Item3)
            .IgnoreNullValues(true);

        TypeAdapterConfig<FilledRuleModel, Rule>
            .NewConfig()
            .TwoWays()
            .Map(dest => dest.IssueField, src => src.IssueField)
            .Map(dest => dest.RuleSubstring, src => src.Rule)
            .IgnoreNonMapped(false);

        TypeAdapterConfig<FilledRuleSetModel, RuleSet>
            .NewConfig()
            .Map(dest => dest.IssueType, src => src.IssueType)
            .Map(dest => dest.Rules, src => src.Rules);
    }

    public static void ConfigureXmlToModelMapping(this IApplicationBuilder app)
    {
        DateTime dateTime;

        TypeAdapterConfig<XmlIssue, IssueModel>
            .NewConfig()
            .Map(dest => dest.Id, src => int.Parse(src.Id))
            .Map(
                dest => dest.TimeCreated,
                src =>
                    src.TimeCreated == null
                        ? DateTime.UtcNow
                        : !DateTime.TryParse(src.TimeCreated, out dateTime)
                            ? DateTime.UtcNow
                            : dateTime.ToUniversalTime()
            )
            .Map(dest => dest.Author, src => src.Author)
            .Map(dest => dest.Company, src => src.Company)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Priority, src => src.Priority)
            .Map(dest => dest.AssignedGroup, src => src.AssignedGroup)
            .Map(dest => dest.Assignee, src => src.Assignee)
            .Map(dest => dest.ShortDescription, src => src.ShortDescription)
            .Map(dest => dest.Description, src => src.Description)
            .IgnoreNullValues(true)
            .IgnoreNonMapped(true);
    }
}