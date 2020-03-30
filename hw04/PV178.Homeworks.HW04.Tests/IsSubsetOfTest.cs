using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PV178.Homeworks.HW04.Tests
{
    [TestClass]
    public class IsSubsetOfTest
    {
        [TestMethod]
        public void TestIsSubsetOf_ProperNonemptySubset()
        {
            var superset = new List<int>() { 1, 2, 3 };
            var subset = new List<int>() { 1 };

            Assert.IsTrue(subset.IsSubsetOf(superset));
        }

        [TestMethod]
        public void TestIsSubsetOf_SetIsOwnSubset()
        {
            var set = new List<int>() { 1, 2, 3 };

            Assert.IsTrue(set.IsSubsetOf(set));
        }

        [TestMethod]
        public void TestIsSubsetOf_EmptyIsSubsetOfSet()
        {
            var set = new List<int>() { 1, 2, 3 };
            var empty = new List<int>();

            Assert.IsTrue(empty.IsSubsetOf(set));
        }

        [TestMethod]
        public void TestIsSubsetOf_EmptyIntersection()
        {
            var set1 = new List<int>() { 1, 2, 3 };
            var set2 = new List<int>() { 4, 5 };

            Assert.IsFalse(set1.IsSubsetOf(set2));
        }

        [TestMethod]
        public void TestIsSubsetOf_SwappedArgs()
        {
            var superset = new List<int>() { 1, 2, 3 };
            var subset = new List<int>() { 1 };

            Assert.IsFalse(superset.IsSubsetOf(subset));
        }
    }
}
