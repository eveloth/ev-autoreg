using Api.Contracts;
using Api.Domain;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;

namespace Api.Services;

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

        var permissions = await _unitofWork.PermissionRepository.GetAll(filter, cts);

        var result = _mapper.Map<IEnumerable<Permission>>(permissions);
        return result;
    }
}