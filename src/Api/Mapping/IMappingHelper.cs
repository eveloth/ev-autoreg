using Api.Domain;
using DataAccessLibrary.Models;

namespace Api.Mapping;

public interface IMappingHelper
{
    Task<User> JoinUserRole(UserModel userModel, CancellationToken cts);
    Task<Rule> JoinIssueTypeAndField(RuleModel ruleModel, CancellationToken cts);
    Task<Issue> JoinIssueTypeAndUser(IssueModel issueModel, CancellationToken cts);
    Task<QueryParameters> JoinIssueType(QueryParametersModel model, CancellationToken cts);
}