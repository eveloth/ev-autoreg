﻿using System.Data.Common;
using DataAccessLibrary.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataAccessLibrary.Repository;

public sealed class UnitofWork : IUnitofWork
{
    private readonly ILogger<UnitofWork> _logger;
    private readonly DbTransaction _transaction;

    public IUserRepository UserRepository { get; set; }
    public IRoleRepository RoleRepository { get; set; }
    public IPermissionRepository PermissionRepository { get; set; }
    public IRolePermissionRepository RolePermissionRepository { get; set; }
    public IGeneralPurposeRepository GpRepository { get; set; }
    public IExtCredentialsRepository ExtCredentialsRepository { get; set; }
    public IIssueTypeRepository IssueTypeRepository { get; set; }
    public IIssueRepository IssueRepository { get; set; }
    public IRuleRepository RuleRepository { get; set; }
    public IEvApiQueryParametersRepository EvApiQueryParametersRepository { get; set; }
    public IMailAnalysisRulesRepository MailAnalysisRulesRepository { get; set; }
    public IIssueFieldRepository IssueFieldRepository { get; set; }

    public UnitofWork(
        ILogger<UnitofWork> logger,
        DbTransaction transaction,
        IUserRepository userRepository,
        IGeneralPurposeRepository gpRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IExtCredentialsRepository extCredentialsRepository,
        IIssueTypeRepository issueTypeRepository,
        IIssueRepository issueRepository,
        IRuleRepository ruleRepository,
        IEvApiQueryParametersRepository evApiQueryParametersRepository,
        IMailAnalysisRulesRepository mailAnalysisRulesRepository,
        IIssueFieldRepository issueFieldRepository
    )
    {
        _logger = logger;
        _transaction = transaction;
        UserRepository = userRepository;
        GpRepository = gpRepository;
        RoleRepository = roleRepository;
        PermissionRepository = permissionRepository;
        RolePermissionRepository = rolePermissionRepository;
        ExtCredentialsRepository = extCredentialsRepository;
        IssueTypeRepository = issueTypeRepository;
        IssueRepository = issueRepository;
        RuleRepository = ruleRepository;
        EvApiQueryParametersRepository = evApiQueryParametersRepository;
        MailAnalysisRulesRepository = mailAnalysisRulesRepository;
        IssueFieldRepository = issueFieldRepository;
    }

    public async Task CommitAsync(CancellationToken cts)
    {
        try
        {
            await _transaction.CommitAsync(cts);

            if (_transaction.Connection is not null)
            {
                await _transaction.Connection.BeginTransactionAsync(cts);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning("An error occured commiting transaction: {ErrorMesage}", e);
            await _transaction.RollbackAsync(cts);
            throw;
        }
    }
}
