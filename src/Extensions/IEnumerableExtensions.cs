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

    /// <summary>
    /// Produces the set difference of two sequences using specified property.
    /// </summary>
    public static IEnumerable<TSource> ExceptByProperty<TSource, TKey>(
        this IEnumerable<TSource> source,
        IEnumerable<TSource> compareTo,
        Func<TSource,  TKey> selector
    )
    {
        return source.ExceptBy(compareTo.Select(selector), selector);
    }

    /// <summary>
    /// The opposite of Any()
    /// </summary>
    public static bool No<TSource>(this IEnumerable<TSource> source)
    {
        return !source.Any();
    }

    /// <summary>
    /// The opposite of Any()
    /// </summary>
    public static bool No<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return !source.Any(predicate);
    }
}