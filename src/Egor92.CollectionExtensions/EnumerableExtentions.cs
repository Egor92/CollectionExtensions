using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Egor92.CollectionExtensions
{
    public static class EnumerableExtentions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static void DisposeEnumerable(this IEnumerable enumerable)
        {
            enumerable.OfType<IDisposable>().ForEach(x => x.Dispose());

            var disposable = enumerable as IDisposable;
            disposable?.Dispose();
        }
    }
}
