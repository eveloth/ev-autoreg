using Api.Contracts.Dto;
using Api.Domain;
using Mapster;

namespace Api.Mapping;

public static class DomainToDtoMappingConfig
{
    public static void ConfigureDomainToDtoMapping(this IApplicationBuilder app)
    {
#pragma warning disable CS8602
        TypeAdapterConfig<Issue, IssueDto>
            .NewConfig()
            .Map(dest => dest.RegistrarId, src => src.Registrar.Id)
            .Map(dest => dest.RegistrarFirstName, src => src.Registrar.FirstName)
            .Map(dest => dest.RegistrarLastName, src => src.Registrar.LastName)
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
#pragma warning restore CS8602

        TypeAdapterConfig<Rule, RuleDto>
            .NewConfig()
            .Map(dest => dest.Rule, src => src.RuleSubstring)
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
    }
}