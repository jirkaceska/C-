using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PV178.Homeworks.HW04.Tests
{
    public struct TestItem
    {
        public int Id { get; }
        public int? NullableId { get; }
        public string Name { get; }

        public TestItem(int id, int? nullableId, string name)
        {
            Id = id;
            NullableId = nullableId;
            Name = name;
        }
    }

    [TestClass]
    public class ZipJoinTest
    {
        #region JoinOnId
        [TestMethod]
        public void TestZipJoin_SameIdsInBothLists_OnId()
        {
            var outer = new List<TestItem> {
                new TestItem(1, null, "Pepa"),
                new TestItem(1, null, "Lojza"),
                new TestItem(2, null, "Franta")
            };
            var inner = new List<TestItem> {
                new TestItem(1, null, "Novák"),
                new TestItem(2, null, "Dvořák")
            };

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_SameIdsInBothListsNotSorted_OnId()
        {
            var outer = new List<TestItem> {
                new TestItem(2, null, "Franta"),
                new TestItem(1, null, "Lojza"),
                new TestItem(1, null, "Pepa"),  
            };
            var inner = new List<TestItem> {
                new TestItem(1, null, "Novák"),
                new TestItem(2, null, "Dvořák")
            };

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_FirstEmpty_OnId()
        {
            var outer = new List<TestItem>();
            var inner = new List<TestItem> {
                new TestItem(1, null, "Novák"),
                new TestItem(2, null, "Dvořák")
            };

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_SecondEmpty_OnId()
        {
            var outer = new List<TestItem> {
                new TestItem(1, null, "Pepa"),
                new TestItem(1, null, "Lojza"),
                new TestItem(2, null, "Franta")
            };
            var inner = new List<TestItem>();

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_PartlyDifferentIds_OnId()
        {
            var outer = new List<TestItem> {
                new TestItem(1, null, "Pepa"),
                new TestItem(2, null, "Lojza"),
                new TestItem(3, null, "Franta")
            };
            var inner = new List<TestItem> {
                new TestItem(2, null, "Novák"),
                new TestItem(3, null, "Dvořák"),
                new TestItem(4, null, "Smetana")
            };

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_CompletelyDifferentIds_OnId()
        {
            var outer = new List<TestItem> {
                new TestItem(1, null, "Pepa"),
                new TestItem(2, null, "Lojza"),
                new TestItem(3, null, "Franta")
            };
            var inner = new List<TestItem> {
                new TestItem(4, null, "Novák"),
                new TestItem(5, null, "Dvořák"),
                new TestItem(6, null, "Smetana")
            };

            DoTest(outer, inner, IdSelector);
        }
        #endregion

        #region JoinOnNullableId
        [TestMethod]
        public void TestZipJoin_SameIdsInBothLists_OnNullableId()
        {
            var outer = new List<TestItem> {
                new TestItem(0, null, "Pepa"),
                new TestItem(0, null, "Lojza"),
                new TestItem(0, 1, "Franta")
            };
            var inner = new List<TestItem>
            {
                new TestItem(0, 1, "Novák"),
                new TestItem(0, null, "Dvořák")
            };

            DoTest(outer, inner, NullableIdSelector);
        }

        [TestMethod]
        public void TestZipJoin_SameIdsInBothListsNotSorted_OnNullableId()
        {
            var outer = new List<TestItem> {
                new TestItem(0, 1, "Franta"),
                new TestItem(0, null, "Lojza"),
                new TestItem(0, null, "Pepa"),
            };
            var inner = new List<TestItem>
            {
                new TestItem(0, 1, "Novák"),
                new TestItem(0, null, "Dvořák")
            };

            DoTest(outer, inner, NullableIdSelector);
        }

        [TestMethod]
        public void TestZipJoin_FirstEmpty_OnNullableId()
        {
            var outer = new List<TestItem>();
            var inner = new List<TestItem> {
                new TestItem(0, null, "Novák"),
                new TestItem(0, 1, "Dvořák")
            };

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_SecondEmpty_OnNullableId()
        {
            var outer = new List<TestItem> {
                new TestItem(0, null, "Pepa"),
                new TestItem(0, 1, "Lojza"),
                new TestItem(0, 2, "Franta")
            };
            var inner = new List<TestItem>();

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_PartlyDifferentIds_OnNullableId()
        {
            var outer = new List<TestItem> {
                new TestItem(0, null, "Pepa"),
                new TestItem(0, 1, "Lojza"),
                new TestItem(0, 2, "Franta")
            };
            var inner = new List<TestItem> {
                new TestItem(0, 2, "Novák"),
                new TestItem(0, 3, "Dvořák"),
                new TestItem(0, 4, "Smetana")
            };

            DoTest(outer, inner, IdSelector);
        }

        [TestMethod]
        public void TestZipJoin_CompletelyDifferentIds_OnNullableId()
        {
            var outer = new List<TestItem> {
                new TestItem(0, null, "Pepa"),
                new TestItem(0, 1, "Lojza"),
                new TestItem(0, 2, "Franta")
            };
            var inner = new List<TestItem> {
                new TestItem(0, 3, "Novák"),
                new TestItem(0, 4, "Dvořák"),
                new TestItem(0, 5, "Smetana")
            };

            DoTest(outer, inner, IdSelector);
        }
        #endregion

        private static int IdSelector(TestItem item)
        {
            return item.Id;
        }

        private static int? NullableIdSelector(TestItem item)
        {
            return item.NullableId;
        }

        private void DoTest<T>(
            IEnumerable<TestItem> outer, 
            IEnumerable<TestItem> inner, 
            Func<TestItem, T> selector   
        )
        {
            var expectedOutput = outer.MakeJoin(inner, selector).OrderBy(Linq.Id);
            var actualOutput = outer.MakeZipJoin(inner, selector).OrderBy(Linq.Id);

            Assert.IsTrue(expectedOutput.SequenceEqual(actualOutput), PrintLists(expectedOutput, actualOutput));
        }

        private string PrintLists(IEnumerable<string> expected, IEnumerable<string> actual)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Expected:");
            builder.AppendLine(String.Join(", ", expected));
            builder.AppendLine();
            builder.AppendLine("Actual:");
            builder.AppendLine(String.Join(", ", actual));
            return builder.ToString();
        }
    }

    public static class JoinHelper
    {
        private static string ResultSelector(TestItem inner, TestItem outer) => $"{inner.Name} {outer.Name}";

        public static IEnumerable<string> MakeJoin<TId>(
            this IEnumerable<TestItem> outer,
            IEnumerable<TestItem> inner,
            Func<TestItem, TId> selector
        ) => outer.Join(inner, selector, selector, ResultSelector);

        public static IEnumerable<string> MakeZipJoin<TId>(
            this IEnumerable<TestItem> outer,
            IEnumerable<TestItem> inner,
            Func<TestItem, TId> selector
        ) => outer.ZipJoin(inner, selector, selector, ResultSelector);
    }
}