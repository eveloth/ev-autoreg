using Api.Domain;
using Api.Exceptions;
using Api.Services.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

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