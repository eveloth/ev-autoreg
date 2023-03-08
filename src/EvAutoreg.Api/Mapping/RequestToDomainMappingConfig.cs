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
            .Map(dest => dest.IssueField.Id, src => src.IssueFieldId)
            .IgnoreNonMapped(false);

        TypeAdapterConfig<RuleSetRequest, RuleSet>
            .NewConfig()
            .Map(dest => dest.IssueType.Id, src => src.IssueTypeId)
            .IgnoreNonMapped(false);

        TypeAdapterConfig<QueryParametersRequest, QueryParameters>
            .NewConfig()
            .Map(dest => dest.IssueType, src => new IssueType())
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
    }
}