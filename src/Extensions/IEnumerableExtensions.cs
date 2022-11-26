namespace Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<List<TSource>> GroupByIntoList<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector
    )
    {
        var groups = source.GroupBy(keySelector);

        return groups.Select(x => x.Select(y => y).ToList());
    }
}
