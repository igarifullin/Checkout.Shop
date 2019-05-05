using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
            return collection;
        }

        public static async Task<IEnumerable<T>> ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> action)
        {
            await Task.WhenAll(tasks: collection.Select(action)).ConfigureAwait(false);
            return collection;
        }

        public static async Task<IEnumerable<TResult>> ForEachAsync<T, TResult>(this IEnumerable<T> collection, Func<T, Task<TResult>> action)
        {
            return await Task.WhenAll(tasks: collection.Select(action));
        }
    }
}