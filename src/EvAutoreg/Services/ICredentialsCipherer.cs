using EvAutoreg.Contracts.Dto.Abstractions;

namespace EvAutoreg.Services;

public interface ICredentialsCipherer
{
    Task<int> CipherAndSave(ExtCredentialsDto credentials, CancellationToken cts);
    Task<ExtCredentialsDto> DecipherAndLoad(int userId, CancellationToken cts);

}