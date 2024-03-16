namespace Playground.Utils.Extensions;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) => source ?? [];

    public static List<T> EmptyIfNull<T>(this List<T> list) => list ?? [];

    public static T[] EmptyIfNull<T>(this T[] arr) => arr ?? [];
}