using System.Text.RegularExpressions;
using Autoregistrar.Domain;
using DataAccessLibrary.Models;
using Mapster;

namespace Autoregistrar.Mapping;

public static class MappingConfig
{
    public static void ConfigureDbToDomainMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<AutoregstrarSettingsModel, AutoregistrarSettings>
            .NewConfig()
            .Map(dest => dest.ExchangeServerUri, src => src.ExchangeServerUri)
            .Map(dest => dest.ExtraViewUri, src => src.ExtraViewUri)
            .Map(dest => dest.NewIssueRegex, src => new Regex(src.NewIssueRegex))
            .Map(dest => dest.IssueNoRegex, src => new Regex(src.IssueNoRegex));

        TypeAdapterConfig<(IssueFieldModel, IEnumerable<RuleModel>), IssueField>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.FieldName, src => src.Item1.FieldName)
            .Map(dest => dest.Rules, src => src.Item2.Select(x => x.IssueFieldId == src.Item1.Id));

        TypeAdapterConfig<(IssueTypeModel, IEnumerable<EvApiQueryParametersModel>), IssueType>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.IssueTypeName, src => src.Item1.IssueTypeName)
            .Map(
                dest => dest.QueryParameters,
                src => src.Item2.First(x => x.IssueTypeId == src.Item1.Id)
            );
    }

    public static void ConfigureXmlToModelMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<XmlIssue, IssueModel>
            .NewConfig()
            .Map(dest => dest.Id, src => int.Parse(src.Id))
            .Map(
                dest => dest.TimeCreated,
                src =>
                    src.TimeCreated == null
                        ? DateTime.UtcNow
                        : DateTime.Parse(src.TimeCreated).ToUniversalTime()
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