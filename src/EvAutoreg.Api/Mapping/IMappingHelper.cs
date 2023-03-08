using EvAutoreg.Api.Domain;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Api.Mapping;

public interface IMappingHelper
{
    Task<User> JoinUserRole(UserModel userModel, CancellationToken cts);
    Task<Issue> JoinIssueTypeAndUser(IssueModel issueModel, CancellationToken cts);
    Task<QueryParameters> JoinIssueType(QueryParametersModel model, CancellationToken cts);
}