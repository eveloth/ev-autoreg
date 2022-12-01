using System.Text.RegularExpressions;

namespace Api;

public partial class ToSlugCaseTransformerConvention : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value is null
            ? null
            : ToSlugCaseTransformerRegex().Replace(value.ToString()!, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex ToSlugCaseTransformerRegex();
}