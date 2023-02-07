using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class AutoregistrarSettingsService : IAutoregistrarSettingsService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public AutoregistrarSettingsService(IMapper mapper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    public async Task<AutoregistrarSettings> Get(CancellationToken cts)
    {
        var settings = await _unitofWork.AutoregistrarSettingsRepository.Get(cts);
        await _unitofWork.CommitAsync(cts);

        if (settings is null)
        {
            throw new ApiException().WithApiError(ErrorCode[9004]);
        }

        var result = _mapper.Map<AutoregistrarSettings>(settings);
        return result;
    }

    public async Task<AutoregistrarSettings> Add(AutoregistrarSettings settings, CancellationToken cts)
    {
        var settingsModel = _mapper.Map<AutoregstrarSettingsModel>(settings);

        var upsertedSettings = await _unitofWork.AutoregistrarSettingsRepository.Upsert(settingsModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<AutoregistrarSettings>(upsertedSettings);
        return result;
    }
}