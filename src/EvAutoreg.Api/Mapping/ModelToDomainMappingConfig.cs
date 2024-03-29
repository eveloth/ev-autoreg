using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Domain;
using EvAutoreg.Data.Models;
using Mapster;

namespace EvAutoreg.Api.Mapping;

public static class ModelToDomainMappingConfig
{
    public static void ConfigureModelToDomainMapping(this IApplicationBuilder app)
    {
        //CS8602 is useless in this context because we're ignoring null values
#pragma warning disable CS8602

        TypeAdapterConfig<(UserModel, RoleModel), User>
            .NewConfig()
            .TwoWays()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.Email, src => src.Item1.Email)
            .Map(dest => dest.FirstName, src => src.Item1.FirstName)
            .Map(dest => dest.LastName, src => src.Item1.LastName)
            .Map(dest => dest.IsBlocked, src => src.Item1.IsBlocked)
            .Map(dest => dest.IsDeleted, src => src.Item1.IsDeleted)
            .Map(dest => dest.Role.Id, src => src.Item2.Id)
            .Map(dest => dest.Role.RoleName, src => src.Item2.RoleName)
            .Map(dest => dest.Role.IsPrivelegedRole, src => src.Item2.IsPrivelegedRole)
            .IgnoreNullValues(true);

        TypeAdapterConfig<List<RolePermissionModel>, RolePermission>
            .NewConfig()
            .Map(dest => dest.RoleId, src => src.First().RoleId)
            .Map(dest => dest.RoleName, src => src.First().RoleName)
            .Map(dest => dest.IsPrivelegedRole, src => src.First().IsPrivelegedRole)
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
                                    PermissionName = x.PermissionName!,
                                    Description = x.Description!,
                                    IsPrivelegedPermission = x.IsPrivelegedPermission
                                }
                        )
            )
            .IgnoreNullValues(true);

        TypeAdapterConfig<(QueryParametersModel, IssueTypeModel?), QueryParameters>
            .NewConfig()
            .TwoWays()
            .Map(dest => dest.IssueType.Id, src => src.Item2.Id)
            .Map(dest => dest.IssueType.IssueTypeName, src => src.Item2.IssueTypeName)
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.WorkTime, src => src.Item1.WorkTime)
            .Map(dest => dest.Status, src => src.Item1.Status)
            .Map(dest => dest.AssignedGroup, src => src.Item1.AssignedGroup)
            .Map(dest => dest.RequestType, src => src.Item1.RequestType)
            .Map(dest => dest.ExecutionOrder, src => src.Item1.ExecutionOrder)
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

        TypeAdapterConfig<(IssueModel, UserModel, IssueTypeModel), Issue>
            .NewConfig()
            .TwoWays()
            .Map(dest => dest.Id, src => src.Item1.Id)
            .Map(dest => dest.TimeCreated, src => src.Item1.TimeCreated)
            .Map(dest => dest.Author, src => src.Item1.Author)
            .Map(dest => dest.Company, src => src.Item1.Company)
            .Map(dest => dest.Status, src => src.Item1.Status)
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
            .Map(dest => dest.IssueType.IssueTypeName, src => src.Item3.IssueTypeName)
            .IgnoreNullValues(true);

#pragma warning restore
    }
}