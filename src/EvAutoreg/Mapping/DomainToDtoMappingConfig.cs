using DataAccessLibrary.Models;
using EvAutoreg.Contracts.Dto;
using Mapster;

namespace EvAutoreg.Mapping;

public static class DomainToDtoMappingConfig
{
    public static void ConfigureDomainToDtoMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<List<RolePermissionModel>, RolePermissionDto>
            .NewConfig()
            .Map(dest => dest.Role.Id, src => src.First().RoleId)
            .Map(dest => dest.Role.RoleName, src => src.First().RoleName)
            .Map(
                dest => dest.Permissions,
                src =>
                    src.First().PermissionId == null
                        ? null
                        : src.Select(
                            x =>
                                new PermissionDto
                                {
                                    Id = x.PermissionId!.Value,
                                    PermissionName = x.PermissionName,
                                    Description = x.Description
                                }
                        )
            );

        TypeAdapterConfig<(RuleModel, IssueTypeModel, IssueFieldModel), RuleDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.Rule, src => src.Item1.Rule)
            .Map(dest => dest.IssueType.Id, src => src.Item2.Id)
            .Map(dest => dest.IssueType.IssueTypeName, src => src.Item2.IssueTypeName)
            .Map(dest => dest.IssueField.Id, src => src.Item3.Id)
            .Map(dest => dest.IssueField.FieldName, src => src.Item3.FieldName)
            .Map(dest => dest.IsRegex, src => src.Item1.IsRegex)
            .Map(dest => dest.IsNegative, src => src.Item1.IsNegative);

        TypeAdapterConfig<(IssueModel, UserProfileModel, IssueTypeModel), IssueDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.TimeCreated, src => src.Item1.TimeCreated)
            .Map(dest => dest.Author, src => src.Item1.Author)
            .Map(dest => dest.Company, src => src.Item1.Company)
            .Map(dest => dest.Priority, src => src.Item1.Priority)
            .Map(dest => dest.AssignedGroup, src => src.Item1.AssignedGroup)
            .Map(dest => dest.Assignee, src => src.Item1.Assignee)
            .Map(dest => dest.ShortDescription, src => src.Item1.ShortDescription)
            .Map(dest => dest.Description, src => src.Item1.Description)
            .Map(dest => dest.Registrar.Id, src => src.Item2.Id)
            .Map(dest => dest.Registrar.Email, src => src.Item2.Email)
            .Map(dest => dest.Registrar.FirstName, src => src.Item2.FirstName)
            .Map(dest => dest.Registrar.LastName, src => src.Item2.LastName)
            .Map(dest => dest.Registrar.IsBlocked, src => src.Item2.IsBlocked)
            .Map(dest => dest.Registrar.IsDeleted, src => src.Item2.IsDeleted)
            .Map(dest => dest.IssueType.Id, src => src.Item3.Id)
            .Map(dest => dest.IssueType.IssueTypeName, src => src.Item3.IssueTypeName);
    }
}