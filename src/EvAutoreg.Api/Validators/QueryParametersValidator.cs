using EvAutoreg.Api.Contracts.Requests;
using FluentValidation;

namespace EvAutoreg.Api.Validators;

public class QueryParametersValiadtor : AbstractValidator<QueryParametersRequest>
{
    public QueryParametersValiadtor()
    {
        RuleFor(x => x.WorkTime).NotNull().NotEmpty().MaximumLength(64);
        RuleFor(x => x.Status).NotNull().NotEmpty().MaximumLength(64);
        RuleFor(x => x.ExecutionOrder).NotNull().GreaterThan(0);
    }
}