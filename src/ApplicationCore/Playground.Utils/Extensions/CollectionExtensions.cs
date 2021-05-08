﻿namespace Playground.Utils.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

        public static List<T> EmptyIfNull<T>(this List<T> list) => list ?? new List<T>();

        public static T[] EmptyIfNull<T>(this T[] arr) => arr ?? Array.Empty<T>();
    }
}