using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sprint.Burndown.WebApp.Utils;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(action, nameof(action));

            foreach (var element in source)
            {
                action(element);
            }
        }

    public static int FirstIndexFor<T>(this IEnumerable<T> source, Func<T,Boolean> condition)
    {
        Guard.IsNotNull(source, nameof(source));
        Guard.IsNotNull(condition, nameof(condition));

        var index = 0;
        foreach (var element in source)
        {
            if (condition(element))
            {
                return index;
            }

            index += 1;
        }

        return -1;
    }

    
}
}
