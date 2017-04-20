using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Egor92.CollectionExtensions.Tests
{
    [TestFixture]
    public class EnumerableExtentionsTest
    {
        [Test]
        public void ForEach_WhenEnumerableHasItems_ThenActionWillBeInvokedForEachItem()
        {
            var itemCount = 1000;
            var originalEnumerable = Enumerable.Range(0, itemCount)
                                               .Select(_ => new object())
                                               .ToList();

            List<object> finalEnumerable = new List<object>(itemCount);
            originalEnumerable.ForEach(x => finalEnumerable.Add(x));

            CollectionAssert.AreEqual(finalEnumerable, originalEnumerable);
        }

        [Test]
        public void ForEach_WhenEnumerableEqualsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((IEnumerable<object>) null).ForEach(_ =>
                {
                });
            });
        }

        [Test]
        public void ForEach_WhenActionEqualsNull_ThenThrowsAnException()
        {
            IEnumerable<object> originalEnumerable = Enumerable.Empty<object>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                originalEnumerable.ForEach(null);
            });
        }

        [Test]
        public void DisposeEnumerable_WhenEnumerableHasItem_ThenDisposeWillBeInvokedForEachItem()
        {
            var itemCount = 1000;
            Func<Mock<IDisposable>> createMockOfDisposable = () =>
            {
                var mock = new Mock<IDisposable>(MockBehavior.Loose);
                mock.Setup(x => x.Dispose());
                return mock;
            };

            var originalMocks = Enumerable.Range(0, itemCount)
                                          .Select(_ => createMockOfDisposable())
                                          .ToList();
            var originalEnumerable = originalMocks.Select(x => x.Object)
                                                  .ToList();

            originalEnumerable.DisposeEnumerable();


            foreach (var mock in originalMocks)
            {
                mock.Verify(x => x.Dispose(), Times.Once);
            }
        }

        [Test]
        public void DisposeEnumerable_WhenEnumerableEqualsNull_ThenThrowsAnException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((IEnumerable<object>) null).DisposeEnumerable();
            });
        }
    }
}
