using System;
using System.Collections.Generic;
using System.Linq;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Utils;

namespace Sprint.Burndown.WebApp.Extensions
{
    public static class ModelAdditionsExtension
    {
        public static void Combine<T>(this IEnumerable<IHasAdditions<T>> models, IEnumerable<T> additions)
            where T : class, IHasIdentifier, new()
        {
            Guard.IsNotNull(models, nameof(models));
            Guard.IsNotNull(additions, nameof(additions));

            models.ForEach(m => m.Combine(additions.FirstOrDefault(p => p.Id == m.Id)));
        }

        public static void Combine<T>(this IHasAdditions<T> model, T additions)
            where T : class, IHasIdentifier, new()
        {
            Guard.IsNotNull(model, nameof(model));

            if (additions == null)
            {
                additions = new T();
            }

            model.Additions = additions;
            additions.Id = model.Id;
        }
    }
}