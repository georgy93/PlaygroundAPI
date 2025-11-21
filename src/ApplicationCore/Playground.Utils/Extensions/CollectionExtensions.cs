namespace Playground.Utils.Extensions;

public static class CollectionExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
    {
        public bool IsNullOrEmpty() => source == null || !source.Any();

        public IEnumerable<TSource> EmptyIfNull() => source ?? [];
    }

    extension<TSource>(List<TSource> source)
    {
        public List<TSource> EmptyIfNull<T>() => source ?? [];
    }

    extension<TSource>(TSource[] source)
    {
        public TSource[] EmptyIfNull<T>() => source ?? [];
    }
}