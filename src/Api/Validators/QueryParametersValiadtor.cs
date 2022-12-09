using Api.Contracts.Requests;
using FluentValidation;

namespace Api.Validators;

public class QueryParametersValiadtor : AbstractValidator<EvApiQueryParametersRequest>
{
    public QueryParametersValiadtor()
    {
        RuleFor(x => x.WorkTime).NotNull().NotEmpty().MaximumLength(64);
        RuleFor(x => x.RegStatus).NotNull().NotEmpty().MaximumLength(64);
    }
}