using Api.Contracts.Requests;
using Api.Domain;
using Mapster;

namespace Api.Mapping;

public static class RequestToDomainMappingConfig
{
    public static void ConfigureRequestToDomainMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<RuleRequest, Rule>
            .NewConfig()
            .Map(dest => dest.IssueType.Id, src => src.IssueTypeId)
            .Map(dest => dest.IssueField.Id, src => src.IssueFieldId)
            .Ignore(dest => dest.IssueType.IssueTypeName)
            .Ignore(dest => dest.IssueField.FieldName)
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
    }
}