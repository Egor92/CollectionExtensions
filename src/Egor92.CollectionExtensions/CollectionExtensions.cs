using System;
using System.Collections.Generic;
using System.Linq;

namespace Egor92.CollectionExtensions
{
    public static class CollectionExtensions
    {
        public static void Sort<T>(this ICollection<T> collection, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            var list = new List<T>(collection);
            list.Sort(comparer);
            collection.ReplaceItems(list);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            foreach (var newItem in newItems)
                collection.Add(newItem);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> removingItems)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (removingItems == null)
                throw new ArgumentNullException(nameof(removingItems));

            foreach (var removingItem in removingItems)
                collection.Remove(removingItem);
        }

        public static void RemoveIf<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            var itemsToRemove = collection.Where(condition)
                                          .ToList();
            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> collection,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> getItem,
                                                                      UpdateDelegate<TItem, TNewItem> updateItem,
                                                                      IEqualityComparer<TKey> comparer = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));
            if (getItemKey == null)
                throw new ArgumentNullException(nameof(getItemKey));
            if (getNewItemKey == null)
                throw new ArgumentNullException(nameof(getNewItemKey));
            if (getItem == null)
                throw new ArgumentNullException(nameof(getItem));
            if (updateItem == null)
                throw new ArgumentNullException(nameof(updateItem));

            comparer = comparer ?? EqualityComparer<TKey>.Default;
            var oldItemsByKey = collection.ToLookup(getItemKey, comparer)
                                          .ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var newItemsByKey = newItems.ToLookup(getNewItemKey, comparer)
                                        .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var itemsToDelete = oldItemsByKey.Where(x => !newItemsByKey.Keys.Contains(x.Key, comparer))
                                             .Select(x => x.Value)
                                             .ToList();
            var itemsToAdd = newItemsByKey.Where(x => !oldItemsByKey.Keys.Contains(x.Key))
                                          .Select(x => getItem(x.Value))
                                          .ToList();

            var oldAndNewItemPairsToUpdate = newItemsByKey.Where(x => oldItemsByKey.Keys.Contains(x.Key))
                                                          .ToDictionary(x => oldItemsByKey[x.Key], x => x.Value);

            collection.RemoveRange(itemsToDelete);
            collection.AddRange(itemsToAdd);

            foreach (var pair in oldAndNewItemPairsToUpdate)
            {
                var oldItem = pair.Key;
                var newItem = pair.Value;
                if (!Equals(oldItem, null) && !Equals(newItem, null))
                {
                    updateItem(oldItem, newItem);
                }
            }
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> collection,
                                                                IEnumerable<TNewItem> newItems,
                                                                Func<TNewItem, TItem> newItemToSourceItemFunc,
                                                                UpdateDelegate<TItem, TNewItem> updateItem,
                                                                IEqualityComparer<TItem> comparer = null)
        {
            Func<TNewItem, TItem> createItemAction = x =>
            {
                var item = newItemToSourceItemFunc(x);
                updateItem(item, x);
                return item;
            };

            AddOrRemoveOrUpdate(collection, newItems, x => x, newItemToSourceItemFunc, createItemAction, updateItem, comparer);
        }

        public static void AddOrRemoveOrUpdate<T>(this ICollection<T> collection,
                                                  IEnumerable<T> newItems,
                                                  UpdateDelegate<T, T> updateItem,
                                                  IEqualityComparer<T> comparer = null)
        {
            AddOrRemoveOrUpdate(collection, newItems, x => x, updateItem, comparer);
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> collection,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> getItem,
                                                                      IEqualityComparer<TKey> comparer = null)
            where TItem : IUpdatable<TNewItem>
        {
            var updateItem = GetUpdateItemAction<TItem, TNewItem>();
            AddOrRemoveOrUpdate(collection, newItems, getItemKey, getNewItemKey, getItem, updateItem, comparer);
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> collection,
                                                                IEnumerable<TNewItem> newItems,
                                                                Func<TNewItem, TItem> newItemToSourceItemFunc,
                                                                IEqualityComparer<TItem> comparer = null)
            where TItem : IUpdatable<TNewItem>
        {
            var updateItem = GetUpdateItemAction<TItem, TNewItem>();
            AddOrRemoveOrUpdate(collection, newItems, newItemToSourceItemFunc, updateItem, comparer);
        }

        private static UpdateDelegate<TTarget, TSource> GetUpdateItemAction<TTarget, TSource>()
            where TTarget : IUpdatable<TSource>
        {
            return (target, source) => target.Update(source);
        }

        public static void AddOrRemove<TItem, TNewItem>(this ICollection<TItem> collection,
                                                        IEnumerable<TNewItem> newItems,
                                                        Func<TNewItem, TItem> getItem,
                                                        IEqualityComparer<TItem> comparer = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (getItem == null)
                throw new ArgumentNullException(nameof(getItem));

            var convertedNewItems = newItems.Select(getItem)
                                            .ToList();

            var itemsToRemove = collection.Except(convertedNewItems, comparer)
                                          .ToList();
            var itemsToAdd = convertedNewItems.Except(collection, comparer)
                                              .ToList();

            collection.RemoveRange(itemsToRemove);
            collection.AddRange(itemsToAdd);
        }

        public static void AddOrRemove<TItem>(this ICollection<TItem> collection,
                                              IEnumerable<TItem> newItems,
                                              IEqualityComparer<TItem> comparer = null)
        {
            AddOrRemove(collection, newItems, x => x, comparer);
        }

        public static void ReplaceItems<TItem, TNewItem>(this ICollection<TItem> collection,
                                                         IEnumerable<TNewItem> newItems,
                                                         Func<TNewItem, TItem> createItemAction)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));
            if (createItemAction == null)
                throw new ArgumentNullException(nameof(createItemAction));

            collection.Clear();
            var convertedNewItems = newItems.Select(createItemAction);
            collection.AddRange(convertedNewItems);
        }

        public static void ReplaceItems<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> newItems)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (newItems == null)
                throw new ArgumentNullException(nameof(newItems));

            collection.ReplaceItems(newItems, x => x);
        }
    }
}
