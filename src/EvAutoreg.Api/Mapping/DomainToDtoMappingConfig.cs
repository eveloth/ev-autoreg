using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Domain;
using Mapster;

namespace EvAutoreg.Api.Mapping;

public static class DomainToDtoMappingConfig
{
    public static void ConfigureDomainToDtoMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<RolePermission, RolePermissionDto>
            .NewConfig()
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);

#pragma warning disable CS8602
        TypeAdapterConfig<Issue, IssueDto>
            .NewConfig()
            .Map(dest => dest.RegistrarId, src => src.Registrar.Id)
            .Map(dest => dest.RegistrarFirstName, src => src.Registrar.FirstName)
            .Map(dest => dest.RegistrarLastName, src => src.Registrar.LastName)
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
#pragma warning restore CS8602
    }
}