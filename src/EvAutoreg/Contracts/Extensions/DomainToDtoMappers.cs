using DataAccessLibrary.Models;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Contracts.Dto.Abstractions;

namespace EvAutoreg.Contracts.Extensions;

public static class DomainToDtoMappers
{
    public static UserDto ToUserDto(this UserModel user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsBlocked = user.IsBlocked,
            IsDeleted = user.IsDeleted,
            RoleId = user.RoleId
        };
    }

    public static UserProfileDto ToUserProfileDto(this UserProfileModel profile)
    {
        RoleDto? role;

        if (profile.Role is null)
        {
            role = null;
        }
        else
        {
            role = new RoleDto
            {
                Id = profile.Role.Id,
                RoleName = profile.Role.RoleName
            };
        }

        return new UserProfileDto
        {
            Id = profile.Id,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            IsBlocked = profile.IsBlocked,
            IsDeleted = profile.IsDeleted,
            Role = role
        };
    }

    public static IEnumerable<UserProfileDto> ToUserProfileCollection(this IEnumerable<UserProfileModel> profiles)
    {
        return profiles.Select(profile => profile.ToUserProfileDto());
    }

    public static RoleDto ToRoleDto(this RoleModel role)
    {
        return new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName
        };
    }
    
    public static IEnumerable<RoleDto> ToRoleCollection(this IEnumerable<RoleModel> roles)
    {
        return roles.Select(role => role.ToRoleDto());
    }

    public static PermissionDto ToPermissionDto(this PermissionModel permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            PermissionName = permission.PermissionName,
            Description = permission.Description
        };
    }

    public static IEnumerable<PermissionDto> ToPermissionCollection(this IEnumerable<PermissionModel> permissions)
    {
        return permissions.Select(permission => permission.ToPermissionDto());
    }

    public static RolePermissionDto ToRolePermissionDto(this IEnumerable<RolePermissionModel> rp)
    {
        var rpList = rp.ToList();

        return ConvertToRolePermissionModel(rpList);
    }

    public static IEnumerable<RolePermissionDto> ToRolePermissionCollection(this IEnumerable<RolePermissionModel> rps)
    {
        var rpsList = rps.ToList();

        var rpsGroups = rpsList.GroupBy(x => new {x.RoleId, x.RoleName});

        return rpsGroups
            .Select(group => group.Select(x => x).ToList())
            .Select(ConvertToRolePermissionModel)
            .ToList();
    }

    public static ExtCredentialsDto ToExtCredentialsDto(this EvCredentialsModel credentials)
    {
        return new EvCredentialsDto
        {
            UserId = credentials.UserId,
            EncryptedEmail = credentials.EncryptedEmail,
            EncryptedPassword = credentials.EncryptedPassword,
            IV = credentials.IV
        };
    }

    public static ExtCredentialsDto ToExtCredentialsDto(this ExchangeCredentialsModel credentials)
    {
        return new ExchangeCredentialsDto
        {
            UserId = credentials.UserId,
            EncryptedEmail = credentials.EncryptedEmail,
            EncryptedPassword = credentials.EncryptedPassword,
            IV = credentials.IV
        };
    }
    
    private static RolePermissionDto ConvertToRolePermissionModel(
            List<RolePermissionModel> rolePermissionList)
    {
        var result = new RolePermissionDto
        {
            Role = new RoleDto
            {
                Id = rolePermissionList.First().RoleId,
                RoleName = rolePermissionList.First().RoleName
            }
        };
    
        if (rolePermissionList.First().PermissionId is null)
            return result;
    
        foreach (var record in rolePermissionList)
        {
            result.Permissions.Add(
                new PermissionDto
                {
                    Id = record.PermissionId!.Value,
                    PermissionName = record.PermissionName,
                    Description = record.Description
                }
            );
        }
    
        return result;
    }
}