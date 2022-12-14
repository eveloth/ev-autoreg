using Api.Domain;
using DataAccessLibrary.Models;
using Mapster;

namespace Api.Mapping;

public static class DomainToModelMappingConfig
{
    public static void ConfigureDomainToModelMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<RolePermission, RolePermissionModel>
            .NewConfig()
            .Map(dest => dest.RoleId, src => src.Role.Id)
            .Map(dest => dest.RoleName, src => src.Role.RoleName)
            .Map(dest => dest.PermissionId, src => src.Permissions.First().Id)
            .Map(dest => dest.PermissionName, src => src.Permissions.First().PermissionName)
            .Map(dest => dest.Description, src => src.Permissions.First().Description)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Rule, RuleModel>
            .NewConfig()
            .Map(dest => dest.Rule, src => src.RuleSubstring)
            .Map(dest => dest.IssueTypeId, src => src.IssueType.Id)
            .Map(dest => dest.IssueFieldId, src => src.IssueField.Id)
            .IgnoreNonMapped(false)
            .IgnoreNullValues(true);
    }
}