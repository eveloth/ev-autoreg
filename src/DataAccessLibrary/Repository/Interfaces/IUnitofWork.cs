namespace DataAccessLibrary.Repository.Interfaces;

public interface IUnitofWork
{
    IUserRepository UserRepository { get; set; }
    IRoleRepository RoleRepository { get; set; }
    IPermissionRepository PermissionRepository { get; set; }
    IRolePermissionRepository RolePermissionRepository { get; set; }
    IGeneralPurposeRepository GpRepository { get; set; }
    IExtCredentialsRepository ExtCredentialsRepository { get; set; }
    IIssueTypeRepository IssueTypeRepository { get; set; }
    IIssueRepository IssueRepository { get; set; }
    IRuleRepository RuleRepository { get; set; }
    IEvApiQueryParametersRepository EvApiQueryParametersRepository { get; set; }
    IAutoregistrarSettingsRepository AutoregistrarSettingsRepository { get; set; }
    IIssueFieldRepository IssueFieldRepository { get; set; }
    Task CommitAsync(CancellationToken cts);
}