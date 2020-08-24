using System;
using System.Collections.Generic;
using System.Linq;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSTrueRandomizerTest.service
{
    [TestClass]
    public class ItemRandomizerServiceTest
    {
        private Random _random;
        private ItemRandomizerService _service;

        [TestInitialize]
        public void Setup()
        {
            _random = new Random(132);
            _service = new ItemRandomizerService(_random);
        }

        [TestMethod]
        public void TestGetRandomItemIndex()
        {
            const int maxItemNumber = 10;

            int randomItemIndex = _service.GetRandomItemIndex(maxItemNumber);

            Assert.IsTrue(randomItemIndex < 10 && randomItemIndex >= 0);
        }

        [TestMethod]
        public void TestGetRandomItemIndexFailsWhenItemNumberIsZero()
        {
            const int maxItemNumber = 0;

            Assert.ThrowsException<RandomizationException>(() => _service.GetRandomItemIndex(maxItemNumber));
        }

        [TestMethod]
        public void TestGetRandomItemIndexFailsWhenItemNumberIsNegative()
        {
            const int maxItemNumber = -1;

            Assert.ThrowsException<RandomizationException>(() => _service.GetRandomItemIndex(maxItemNumber));
        }

        [TestMethod]
        public void TestGetMultipleRandomEntriesFromList()
        {
            const int numberOfEntriesToGet = 3;
            var sourceList = new List<string>() { "string1", "string2", "string3", "string4", "string5" };

            IEnumerable<string> result = _service.GetRandomEntriesFromList(sourceList, numberOfEntriesToGet);

            HashSet<string> resultSet = result.ToHashSet();
            const int expectedNumberOfUniqueEntries = 3;
            Assert.AreEqual(expectedNumberOfUniqueEntries, resultSet.Count);
        }

        //[TestMethod]
        //public void TestGetMultipleRandomEntriesFromListHandlesRandomNumberCollisions()
        //{
        //    //TODO
        //    Assert.Fail();
        //}

        [TestMethod]
        public void TestGetSingleRandomEntryFromList()
        {
            const int numberOfEntriesToGet = 1;
            var sourceList = new List<string>() { "string1", "string2", "string3", "string4", "string5" };

            IEnumerable<string> result = _service.GetRandomEntriesFromList(sourceList, numberOfEntriesToGet);

            Assert.AreEqual(numberOfEntriesToGet, result.Count());
            Assert.IsTrue(sourceList.Contains(result.First()));
        }

        [TestMethod]
        public void TestGetZeroRandomEntriesGivesEmptyList()
        {
            const int numberOfEntriesToGet = 0;
            var sourceList = new List<string>() { "string1", "string2", "string3", "string4", "string5" };

            IEnumerable<string> result = _service.GetRandomEntriesFromList(sourceList, numberOfEntriesToGet);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void TestGetNegativeRandomEntriesGivesEmptyList()
        {
            const int numberOfEntriesToGet = -1;
            var sourceList = new List<string>() { "string1", "string2", "string3", "string4", "string5" };

            IEnumerable<string> result = _service.GetRandomEntriesFromList(sourceList, numberOfEntriesToGet);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetRandomEntriesFromEmptyListGivesEmptyList()
        {
            const int numberOfEntriesToGet = 1;
            var emptyList = new List<string>();

            IEnumerable<string> result = _service.GetRandomEntriesFromList(emptyList, numberOfEntriesToGet);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void TestRequestingMoreRandomItemsFromListThanAvailableGivesAllItems()
        {
            const int numberOfEntriesToGet = 3;
            var sourceList = new List<string>() { "string1", "string2"};

            IEnumerable<string> result = _service.GetRandomEntriesFromList(sourceList, numberOfEntriesToGet);

            Assert.AreNotSame(result, sourceList);
            Assert.IsTrue(result.SequenceEqual(sourceList));
        }

        [TestMethod]
        public void GetRandomEntriesFromListDoesNotAlterOriginalList()
        {
            const int numberOfEntriesToGet = 3;
            var sourceList = new List<string>() { "string1", "string2", "string3", "string4", "string5" };
            var expectedAfterCompletion = new List<string>() { "string1", "string2", "string3", "string4", "string5" };

            IEnumerable<string> result = _service.GetRandomEntriesFromList(sourceList, numberOfEntriesToGet);

            Assert.IsTrue(sourceList.SequenceEqual(expectedAfterCompletion));
        }
    }
}