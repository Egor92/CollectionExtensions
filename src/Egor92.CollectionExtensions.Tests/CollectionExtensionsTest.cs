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
            IEnumerable<int> newItems = Enumerable.Range(10, newItemsCount).ToList();

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
                ((ICollection<object>)null).RemoveRange(new List<object>());
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
                ((ICollection<object>)null).RemoveIf(_ => false);
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
    }
}
