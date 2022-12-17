﻿using Api.Domain;
using Api.Services.Interfaces;
using DataAccessLibrary.Repository.Interfaces;

namespace Api.Services;

public class ExtCredentialsService : IExtCredentialsService
{
    private readonly ICredentialsEncryptor _encryptor;
    private readonly IUnitofWork _unitofWork;

    public ExtCredentialsService(ICredentialsEncryptor encryptor, IUnitofWork unitofWork)
    {
        _encryptor = encryptor;
        _unitofWork = unitofWork;
    }

    public async Task<int> SaveExchangeCredentials(
        int id,
        ExchangeCredentials credentials,
        CancellationToken cts
    )
    {
        var encryptedCredentials = _encryptor.EncryptExchangeCredentials(id, credentials);

        var updatedForUserId = await _unitofWork.ExtCredentialsRepository.SaveExchangeCredentials(
            encryptedCredentials,
            cts
        );

        return updatedForUserId;
    }

    public async Task<int> SaveEvCredentials(
        int id,
        EvCredentials credentials,
        CancellationToken cts
    )
    {
        var encryptedCredentials = _encryptor.EncryptEvCredentials(id, credentials);

        var updatedForUserId = await _unitofWork.ExtCredentialsRepository.SaveEvCredentials(encryptedCredentials, cts);

        return updatedForUserId;
    }
}