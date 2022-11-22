using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts.Dto.Abstractions;

namespace EvAutoreg.Services;

public class CredentialsCipherer : ICredentialsCipherer
{
    private readonly IUnitofWork _unitofWork;

    public CredentialsCipherer(IUnitofWork unitofWork)
    {
        _unitofWork = unitofWork;
    }

    public Task<int> CipherAndSave(ExtCredentialsDto credentials, CancellationToken cts)
    {
        throw new NotImplementedException();
    }

    public Task<ExtCredentialsDto> DecipherAndLoad(int userId, CancellationToken cts)
    {
        throw new NotImplementedException();
    }
}