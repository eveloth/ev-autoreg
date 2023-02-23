using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IIssueFieldRepository
{
    Task<IssueFieldModel?> Get(int issueFieldId, CancellationToken cts);
    Task<IEnumerable<IssueFieldModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    );
    Task Add(IssueFieldModel issueField, CancellationToken cts);
    Task Delete(int issueFieldId, CancellationToken cts);
    Task<bool> DoesExist(int issueFieldId, CancellationToken cts);
}