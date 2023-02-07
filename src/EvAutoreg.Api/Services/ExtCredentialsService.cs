using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Repository.Interfaces;

namespace EvAutoreg.Api.Services;

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
        await _unitofWork.CommitAsync(cts);

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
        await _unitofWork.CommitAsync(cts);

        return updatedForUserId;
    }
}