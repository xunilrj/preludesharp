using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreludeSharp;
using System.Linq;

namespace PreludeSharp.Tests
{
    [TestClass]
    public class PreludeSharpTests
    {
        [TestMethod]
        public void AllTests()
        {
            var list1 = new[] { 1, 2, 3 };

            var sum1 = new Sum1();
            var list2 = new Map<int, int>().Invoke(sum1, list1).ToArray();

            Assert.AreEqual(2, list2[0]);
            Assert.AreEqual(3, list2[1]);
            Assert.AreEqual(4, list2[2]);

            var zippedList = new Zip<int, int>().Invoke(list1, list2).ToArray();

            Assert.AreEqual(1, zippedList[0].Item1);
            Assert.AreEqual(2, zippedList[0].Item2);

            var foldrSum = new Foldr<int, int>().Invoke(new Sum(), 0, list1);
            Assert.AreEqual(6, foldrSum);

            var foldlSum = new Foldl<int, int>().Invoke(new Sum(), 0, list1);
            Assert.AreEqual(6, foldlSum);

            var concatList = new Concat<int>().Invoke(new[] { list1, list2 }).ToArray();
            Assert.AreEqual(6, concatList.Count());
            Assert.AreEqual(1, concatList[0]);
            Assert.AreEqual(4, concatList[5]);

            var reversedConcatList = new Reverse<int>().Invoke(concatList).ToList();
            Assert.AreEqual(4, reversedConcatList[0]);
            Assert.AreEqual(1, reversedConcatList[5]);
        }
    }

    public class Sum1 : IFunc<int, int>
    {
        public int Invoke(int item)
        {
            return item + 1;
        }
    }

    public class Sum : IFunc<int, int, int>
    {
        public int Invoke(int a, int b)
        {
            return a + b;
        }
    }
}
