using System.Collections.Generic;
using System.Linq;
using BSTrueRandomizer;
using BSTrueRandomizer.model;
using BSTrueRandomizer.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSTrueRandomizerTest
{
    [TestClass]
    public class TypeRandomizationTests
    {
        private const string FolderPathOutput = "file-resources/ComponentTest/Output/";
        private Options _opts;

        [TestInitialize]
        public void Setup()
        {
            _opts = new Options {OutputPath = FolderPathOutput, IsJsonOnly = true, IsRandomizeType = true};
        }

        [TestMethod]
        public void TestFixedKeyLocationsHaveTypeKey()
        {
            // Assign
            const int expectedRandomKeyLocations = 0;
            _opts.NumberOfKeyLocations = expectedRandomKeyLocations;
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);

            //Assert
            List<DropItemEntry> outputDropList = gameFileReader.ReadAllFiles(FolderPathOutput).DropList;
            AssertKeyTypeForEntryNamed(outputDropList, "Swordsman");
            AssertKeyTypeForEntryNamed(outputDropList, "CertificationboardEvent");
            AssertKeyTypeForEntryNamed(outputDropList, "Treasurebox_SAN024");
        }

        [TestMethod]
        public void TestUserDefinedNumberOfEntriesAreSetAsKeyLocation()
        {
            // Assign
            const int expectedRandomKeyLocations = 5;
            const int expectedFixedKeyLocations = 3;
            _opts.NumberOfKeyLocations = expectedRandomKeyLocations;
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);

            //Assert
            List<DropItemEntry> outputDropList = gameFileReader.ReadAllFiles(FolderPathOutput).DropList;

            int actualNumberOfKeyLocations = GetNumberOfEntriesWithType(outputDropList, Constants.ItemTypeKey);
            int expectedTotalKeyLocations = expectedFixedKeyLocations + expectedRandomKeyLocations;
            Assert.AreEqual(expectedTotalKeyLocations, actualNumberOfKeyLocations);
        }

        [TestMethod]
        public void TestExemptItemTypeEntriesAreUnchanged()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);

            //Assert
            List<DropItemEntry> inputDropList = gameFileReader.ReadAllFiles(_opts.InputPath).DropList;
            List<DropItemEntry> outputDropList = gameFileReader.ReadAllFiles(FolderPathOutput).DropList;

            AssertEntriesWithTypeAreUnchanged(inputDropList, outputDropList, Constants.ItemTypeConsumable);
            AssertEntriesWithTypeAreUnchanged(inputDropList, outputDropList, Constants.ItemTypeCraftingMaterials);
        }

        [TestMethod]
        public void TestNonExemptNonKeyEntriesAllHaveSameType()
        {
            // Assign
            const int expectedRandomKeyLocations = 5;
            const int numberOfTypesThatAreNotChangeable = 3;

            _opts.NumberOfKeyLocations = expectedRandomKeyLocations;
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);

            //Assert
            List<DropItemEntry> outputDropList = gameFileReader.ReadAllFiles(FolderPathOutput).DropList;

            int numberOfValidTypesNames = outputDropList.Where(entry => entry.IsEntryValid()).Distinct()
                .Select(entry => entry.GetItemType())
                .Distinct()
                .Count();
            Assert.AreEqual(numberOfTypesThatAreNotChangeable + 1, numberOfValidTypesNames);
        }

        private static void AssertKeyTypeForEntryNamed(List<DropItemEntry> outputDropList, string entryName)
        {
            string fixedLocationItemType = GetItemTypeForEntryId(outputDropList, entryName);
            Assert.AreEqual(Constants.ItemTypeKey, fixedLocationItemType);
        }

        private static void AssertEntriesWithTypeAreUnchanged(List<DropItemEntry> inputDropList, List<DropItemEntry> outputDropList, string itemType)
        {
            IEnumerable<string> inputIdsForEntriesWithType = inputDropList.Where(entry => itemType.Equals(entry.GetItemType())).Select(entry => entry.Key);
            IEnumerable<string> outputIdsForEntriesWithType = outputDropList.Where(entry => itemType.Equals(entry.GetItemType())).Select(entry => entry.Key);

            Assert.IsTrue(inputIdsForEntriesWithType.SequenceEqual(outputIdsForEntriesWithType));
        }

        private static int GetNumberOfEntriesWithType(IEnumerable<IItemEntry> itemList, string itemType)
        {
            return itemList.Count(entry => itemType.Equals(entry.GetItemType()));
        }

        private static string GetItemTypeForEntryId(IEnumerable<IItemEntry> outputDropList, string entryKey)
        {
            return outputDropList.First(entry => entry.Key.Equals(entryKey)).GetItemType();
        }
    }
}