﻿using System.Text.RegularExpressions;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Data.Models;
using Mapster;

namespace EvAutoreg.Autoregistrar.Mapping;

public static class MappingConfig
{
    public static void ConfigureDbToDomainMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<RuleModel, Rule>
            .NewConfig()
            .Map(dest => dest.RuleSubstring, src => src.Rule)
            .Map(dest => dest.IssueTypeId, src => src.IssueTypeId)
            .Map(dest => dest.IsNegative, src => src.IsNegative)
            .Map(dest => dest.IsRegex, src => src.IsRegex)
            .IgnoreNullValues(true);

        TypeAdapterConfig<AutoregstrarSettingsModel, AutoregistrarSettings>
            .NewConfig()
            .Map(dest => dest.ExchangeServerUri, src => src.ExchangeServerUri)
            .Map(dest => dest.ExtraViewUri, src => src.ExtraViewUri)
            .Map(dest => dest.NewIssueRegex, src => new Regex(src.NewIssueRegex))
            .Map(dest => dest.IssueNoRegex, src => new Regex(src.IssueNoRegex))
            .IgnoreNullValues(true);

        TypeAdapterConfig<(IssueFieldModel, IEnumerable<RuleModel>), IssueField>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.FieldName, src => src.Item1.FieldName)
            .Map(dest => dest.Rules, src => src.Item2.Where(x => x.IssueFieldId == src.Item1.Id))
            .IgnoreNullValues(true);

        TypeAdapterConfig<(IssueTypeModel, IEnumerable<QueryParametersModel>), IssueType>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.IssueTypeName, src => src.Item1.IssueTypeName)
            .Map(dest => dest.QueryParameters, src => src.Item2)
            .IgnoreNullValues(true);
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