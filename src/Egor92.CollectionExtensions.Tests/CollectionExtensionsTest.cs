using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Egor92.CollectionExtensions.Tests
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        private class Item : IUpdatable<Item>
        {
            public int Id { get; set; }

            public string Value { get; set; }

            public void Update(Item source)
            {
                Id = source.Id;
                Value = source.Value;
            }
        }

        [Test]
        public void Sort_WhenEnumerableEqualsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ICollection<object>) null).Sort();
            });
        }

        [Test]
        public void Sort_WhenComparerIsNull_ThenCollectionWillBeSorted()
        {
            var random = new Random();
            IList<double> list = Enumerable.Range(0, 10)
                                           .Select(x => random.NextDouble())
                                           .ToList();
            list.Sort();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1])
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void Sort_WhenComparerIsNotNull_ThenCanSort()
        {
            var random = new Random();
            IList<double> list = Enumerable.Range(0, 10)
                                           .Select(x => random.NextDouble())
                                           .ToList();
            var comparerMock = new Mock<IComparer<double>>(MockBehavior.Loose);
            comparerMock.Setup(x => x.Compare(It.IsAny<double>(), It.IsAny<double>()))
                        .Returns((double x, double y) => Math.Sign(x - y));
            var comparer = comparerMock.Object;
            list.Sort(comparer);
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (comparer.Compare(list[i], list[i + 1]) > 0)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void AddRange_WhenCollectionAndItemsArgAreNotNull_ThenItemsWillBeInCollection()
        {
            const int startCount = 10;
            List<int> list = Enumerable.Range(0, startCount)
                                       .ToList();

            const int newItemsCount = 5;
            IEnumerable<int> newItems = Enumerable.Range(10, newItemsCount)
                                                  .ToList();

            list.AddRange(newItems);

            foreach (var newItem in newItems)
            {
                CollectionAssert.Contains(list, newItem);
            }
        }

        [Test]
        public void AddRange_WhenCollectionIsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ICollection<object>) null).AddRange(new List<object>());
            });
        }

        [Test]
        public void AddRange_WhenNewItemsArgIsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.AddRange(null);
            });
        }

        [Test]
        public void RemoveRange_WhenCollectionAndItemsArgAreNotNull_ThenItemsWillBeRemovedFromCollection()
        {
            const int startCount = 10;
            List<int> list = Enumerable.Range(0, startCount)
                                       .ToList();

            const int oldItemsCount = 5;
            IList<int> oldItems = Enumerable.Range(0, oldItemsCount)
                                            .Select(x => 2 * x)
                                            .ToList();

            list.RemoveRange(oldItems);

            foreach (var newItem in oldItems)
            {
                CollectionAssert.DoesNotContain(list, newItem);
            }
        }

        [Test]
        public void RemoveRange_WhenCollectionIsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ICollection<object>) null).RemoveRange(new List<object>());
            });
        }

        [Test]
        public void AddRange_WhenItemsArgIsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.RemoveRange(null);
            });
        }

        [Test]
        public void RemoveIf_WhenCollectionAndConditionAreNotNull_ThenItemsWhichSatisfyConditionWillBeRemoved()
        {
            const int startCount = 10;
            IList<int> list = Enumerable.Range(0, startCount)
                                        .ToList();

            var condition = new Func<int, bool>(x => x % 3 == 0);

            var leftItemsCount = list.Count(condition);
            var keptItemsCount = list.Count - leftItemsCount;

            list.RemoveIf(condition);

            Assert.AreEqual(keptItemsCount, list.Count);
        }

        [Test]
        public void RemoveIf_WhenCollectionIsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ICollection<object>) null).RemoveIf(_ => false);
            });
        }

        [Test]
        public void RemoveIf_WhenConditionIsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.RemoveIf(null);
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenCollectionIsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((ICollection<object>) null).AddOrRemoveOrUpdate(new List<object>(), x => x, x => x, x => x, (target, source) =>
                {
                });
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenNewItemsArgIsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.AddOrRemoveOrUpdate<object, object, object>(null, x => x, x => x, x => x, (target, source) =>
                {
                });
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenGetItemKeyArgIsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.AddOrRemoveOrUpdate(new List<object>(), null, x => x, x => x, (target, source) =>
                {
                });
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenGetNewItemKeyArgIsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.AddOrRemoveOrUpdate(new List<object>(), x => x, null, x => x, (target, source) =>
                {
                });
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenGetItemArgIsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.AddOrRemoveOrUpdate(new List<object>(), x => x, x => x, null, (target, source) =>
                {
                });
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenUpdateItemArgIsNull_ThenThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var collection = new Collection<object>();
                collection.AddOrRemoveOrUpdate(new List<object>(), x => x, x => x, x => x, (UpdateDelegate<object, object>) null);
            });
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenAllArgsExceptComparerAreNotNull_ThenItemWithNewKeysWillBeAddedToCollection()
        {
            var item1 = new Item()
            {
                Id = 1,
                Value = "First",
            };
            var item2 = new Item()
            {
                Id = 2,
                Value = "Second",
            };
            var item3 = new Item()
            {
                Id = 3,
                Value = "Third",
            };
            var item4 = new Item()
            {
                Id = 4,
                Value = "Forth",
            };
            var item5 = new Item()
            {
                Id = 5,
                Value = "Fifth",
            };

            var originalItems = new Collection<Item>()
            {
                item1,
                item2,
                item3,
            };

            var newItems = new Collection<Item>()
            {
                item3,
                item4,
                item5,
            };

            var collection = new List<Item>(originalItems);
            collection.AddOrRemoveOrUpdate(newItems, x => x.Id, x => x.Id, x => x, (target, source) => target.Update(source));

            CollectionAssert.AreEquivalent(newItems, collection);
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenItemIsInCollectionAndInNewItems_ThenThisItemWillNotBeenAddedOrRemoved()
        {
            var commonItemId = 1;
            var commonItem = new Item()
            {
                Id = commonItemId,
                Value = "CommonItem",
            };

            var originalItems = new Collection<Item>()
            {
                commonItem,
                new Item()
                {
                    Id = 2,
                },
                new Item()
                {
                    Id = 3,
                },
            };

            var newItems = new Collection<Item>()
            {
                commonItem,
                new Item()
                {
                    Id = 4,
                },
                new Item()
                {
                    Id = 5,
                },
            };

            bool isCommonItemChangingNotified = false;
            var collection = new ObservableCollection<Item>(originalItems);
            collection.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Item newItem in e.NewItems)
                    {
                        isCommonItemChangingNotified |= newItem.Id == commonItem.Id;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (Item oldItems in e.OldItems)
                    {
                        isCommonItemChangingNotified |= oldItems.Id == commonItem.Id;
                    }
                }
            };

            collection.AddOrRemoveOrUpdate(newItems, x => x.Id, x => x.Id, x => x, (target, source) => target.Update(source));

            Assert.IsFalse(isCommonItemChangingNotified);
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenSeveralItemsWithSameKeyAreInNewItemsCollection_ThenWillBeAddedFirstItemOnly()
        {
            var originalItems = new Collection<Item>()
            {
                new Item()
                {
                    Id = 1,
                },
                new Item()
                {
                    Id = 2,
                },
                new Item()
                {
                    Id = 3,
                },
            };

            var newItems = new Collection<Item>()
            {
                new Item()
                {
                    Id = 4,
                },
                new Item()
                {
                    Id = 4,
                },
            };

            var collection = new List<Item>(originalItems);
            collection.AddOrRemoveOrUpdate(newItems, x => x.Id, x => x.Id, x => x);

            Assert.AreEqual(1, collection.Count);
            CollectionAssert.Contains(collection, newItems[0]);
        }

        [Test]
        public void AddOrRemoveOrUpdate_WhenTwoItemsWithSameKeyAreInNewItemsCollection_ThenTheFirstItemWillBeUpdationSource()
        {
            var oldValue = "Old value";
            var originalItems = new Collection<Item>()
            {
                new Item()
                {
                    Id = 1,
                    Value = oldValue,
                },
            };

            var newValue = "New value";
            var otherValue = "Other value";
            var newItems = new Collection<Item>()
            {
                new Item()
                {
                    Id = 1,
                    Value = newValue,
                },
                new Item()
                {
                    Id = 1,
                    Value = otherValue,
                },
            };

            var collection = new List<Item>(originalItems);
            collection.AddOrRemoveOrUpdate(newItems, x => x.Id, x => x.Id, x => x);

            Assert.AreEqual(1, collection.Count);
            CollectionAssert.AreEquivalent(collection, originalItems);
            Assert.AreEqual(newValue, collection[0].Value);
        }
    }
}
