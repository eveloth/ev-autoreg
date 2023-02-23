using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;

namespace EvAutoreg.Api.Services;

public class PermissionService : IPermissionService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public PermissionService(IMapper mapper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<Permission>> GetAll(PaginationQuery paginationQuery, CancellationToken cts)
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var permissions = await _unitofWork.PermissionRepository.GetAll(cts, filter);

        var result = _mapper.Map<IEnumerable<Permission>>(permissions);
        return result;
    }
}