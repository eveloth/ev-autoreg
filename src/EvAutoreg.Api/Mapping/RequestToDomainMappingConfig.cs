using EvAutoreg.Api.Contracts.Requests;
using EvAutoreg.Api.Domain;
using Mapster;

namespace EvAutoreg.Api.Mapping;

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

        TypeAdapterConfig<QueryParametersRequest, QueryParameters>
            .NewConfig()
            .Map(dest => dest.IssueType, src => new IssueType())
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
    }
}