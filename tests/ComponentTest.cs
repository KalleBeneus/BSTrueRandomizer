using System.Collections.Generic;
using System.IO;
using System.Linq;
using BSTrueRandomizer;
using BSTrueRandomizer.model;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSTrueRandomizerTest
{
    [TestClass]
    public class ComponentTest
    {

        // TODO add test for food randomization

        private const string FolderPathOutput = "file-resources/ComponentTest/Output/";
        private Options _opts;

        [TestInitialize]
        public void Setup()
        {
            _opts = new Options {OutputPath = FolderPathOutput};
        }

        [TestMethod]
        public void TestAllSingleInstanceUniqueItemsPresentInResultLists()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles inputFiles = gameFileReader.ReadAllFiles(_opts.InputPath);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            int expectedUniqueItemCount = CountAllItemsWithNamesContaining("unique_", inputFiles);
            int actualUniqueItemCount = CountAllItemsWithNamesContaining("unique_", outputFiles);
            Assert.AreEqual(expectedUniqueItemCount, actualUniqueItemCount);
        }

        [TestMethod]
        public void TestThatIfUniqueItemHasSeveralInstancesItCanBeAssignedToSameNumberOfSlots()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestMultipleInstanceUniqueItems\";
            _opts.SeedText = "SeedText-TwoUniquesInDropAndQuestZeroInCraftList";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            int actualDropAndQuestCount = CountItemsWithNamesContaining("unique_weapon_1", outputFiles.DropList);
            actualDropAndQuestCount += CountItemsWithNamesContaining("unique_weapon_1", outputFiles.QuestList);
            int actualCraftCount = CountItemsWithNamesContaining("unique_weapon_1", outputFiles.CraftList);
            Assert.AreEqual(2, actualDropAndQuestCount);
            Assert.AreEqual(0, actualCraftCount);
        }

        [TestMethod]
        public void TestThatIfUniqueItemHasSeveralInstancesItCanBeBothDropAndCraftable()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestMultipleInstanceUniqueItems\";
            _opts.SeedText = "SeedText-1UniqueItemInDropOrQuest1InCraft";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            int actualDropAndQuestCount = CountItemsWithNamesContaining("unique_weapon_1", outputFiles.DropList);
            actualDropAndQuestCount += CountItemsWithNamesContaining("unique_weapon_1", outputFiles.QuestList);
            int actualCraftCount = CountItemsWithNamesContaining("unique_weapon_1", outputFiles.CraftList);
            Assert.AreEqual(1, actualDropAndQuestCount);
            Assert.AreEqual(1, actualCraftCount);
        }

        [TestMethod]
        public void TestThatIfUniqueItemHasSeveralInstancesItWillTakeUpOnlyOneEntryIfAllAreAssignedToCraft()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestMultipleInstanceUniqueItems\";
            _opts.SeedText = "SeedText-MultipleUniquesAreAllAssignedToCrafting";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles inputFiles = gameFileReader.ReadAllFiles(_opts.InputPath);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            int actualDropAndQuestCount = CountItemsWithNamesContaining("unique_weapon_1", outputFiles.DropList);
            actualDropAndQuestCount += CountItemsWithNamesContaining("unique_weapon_1", outputFiles.QuestList);
            int actualCraftCount = CountItemsWithNamesContaining("unique_weapon_1", outputFiles.CraftList);
            Assert.AreEqual(0, actualDropAndQuestCount);
            Assert.AreEqual(1, actualCraftCount);
            int expectedNumberOfEntriesInCraft = inputFiles.CraftList.Count + 1;
            Assert.AreEqual(expectedNumberOfEntriesInCraft, outputFiles.CraftList.Count);
        }

        [TestMethod]
        public void TestThatIfCraftableItemCanBeFoundInMultiplesItCanBeAssignedInMultiples()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestMultipleInstanceUniqueItems\";
            _opts.SeedText = "SeedText-TwoCraftableInDropAndQuestBothSetAsFindable";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            int actualDropAndQuestCount = CountItemsWithNamesContaining("craftable_weapon_1", outputFiles.DropList);
            actualDropAndQuestCount += CountItemsWithNamesContaining("craftable_weapon_1", outputFiles.QuestList);
            Assert.AreEqual(2, actualDropAndQuestCount);
        }

        [TestMethod]
        public void TestResultListsEntryCountIsUnchanged()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles inputFiles = gameFileReader.ReadAllFiles(_opts.InputPath);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            Assert.AreEqual(inputFiles.QuestList.Count, outputFiles.QuestList.Count);
            Assert.AreEqual(inputFiles.DropList.Count, outputFiles.DropList.Count);
            Assert.AreEqual(inputFiles.CraftList.Count, GetResultingCraftListItemCount(outputFiles.CraftList));
        }

        [TestMethod]
        public void TestResultListItemTypesAreUnchanged()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles inputFiles = gameFileReader.ReadAllFiles(_opts.InputPath);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            AssertInputAndOutputItemTypesMatch(inputFiles.DropList, outputFiles.DropList);
            AssertInputAndOutputItemTypesMatch(inputFiles.QuestList, outputFiles.QuestList);
            AssertInputAndOutputItemTypesMatch(inputFiles.CraftList, outputFiles.CraftList);
        }

        [TestMethod]
        public void TestNonValidEntriesAreUnchanged()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestRandomizeDropQuestCraft\";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles inputFiles = gameFileReader.ReadAllFiles(_opts.InputPath);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            AssertInvalidEntriesAreEqual(inputFiles.DropList, outputFiles.DropList);
            AssertInvalidEntriesAreEqual(inputFiles.QuestList, outputFiles.QuestList);
            AssertInvalidEntriesAreEqual(inputFiles.CraftList, outputFiles.CraftList);
        }

        [TestMethod]
        public void TestProvidingNoSeedGivesDifferentResultsOnMultipleRuns()
        {
            // Act
            Program.RunMain(_opts);
            string firstRunDropMasterString = File.ReadAllText(FolderPathOutput + "PB_DT_DropRateMaster.json");
            string firstRunQuestMasterString = File.ReadAllText(FolderPathOutput + "PB_DT_QuestMaster.json");
            string firstRunCraftMasterString = File.ReadAllText(FolderPathOutput + "PB_DT_CraftMaster.json");
            Program.RunMain(_opts);
            string secondRunDropMasterString = File.ReadAllText(FolderPathOutput + "PB_DT_DropRateMaster.json");
            string secondRunQuestMasterString = File.ReadAllText(FolderPathOutput + "PB_DT_QuestMaster.json");
            string secondRunCraftMasterString = File.ReadAllText(FolderPathOutput + "PB_DT_CraftMaster.json");

            //Assert
            Assert.AreNotEqual(firstRunDropMasterString, secondRunDropMasterString);
            Assert.AreNotEqual(firstRunQuestMasterString, secondRunQuestMasterString);
            Assert.AreNotEqual(firstRunCraftMasterString, secondRunCraftMasterString);
        }

        [TestMethod]
        public void TestProvidingSameSeedGivesSameResult()
        {
            // Assign
            _opts.SeedText = "SeedText-FixedSeed";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            string firstRunDropMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_DropRateMaster.json");
            string firstRunQuestMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_QuestMaster.json");
            string firstRunCraftMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_CraftMaster.json");
            Program.RunMain(_opts);
            string secondRunDropMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_DropRateMaster.json");
            string secondRunQuestMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_QuestMaster.json");
            string secondRunCraftMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_CraftMaster.json");

            //Assert
            Assert.AreEqual(firstRunDropMasterString, secondRunDropMasterString);
            Assert.AreEqual(firstRunQuestMasterString, secondRunQuestMasterString);
            Assert.AreEqual(firstRunCraftMasterString, secondRunCraftMasterString);
        }

        [TestMethod]
        public void TestProvidingDifferentSeedsGivesDifferentResults()
        {
            // Assign
            const string firstSeed = "SeedText-First";
            const string secondSeed = "SeedText-Second";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            _opts.SeedText = firstSeed;
            Program.RunMain(_opts);
            string firstRunDropMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_DropRateMaster.json");
            string firstRunQuestMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_QuestMaster.json");
            string firstRunCraftMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_CraftMaster.json");
            _opts.SeedText = secondSeed;
            Program.RunMain(_opts);
            string secondRunDropMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_DropRateMaster.json");
            string secondRunQuestMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_QuestMaster.json");
            string secondRunCraftMasterString = File.ReadAllText(FolderPathOutput + "/PB_DT_CraftMaster.json");

            //Assert
            Assert.AreNotEqual(firstRunDropMasterString, secondRunDropMasterString);
            Assert.AreNotEqual(firstRunQuestMasterString, secondRunQuestMasterString);
            Assert.AreNotEqual(firstRunCraftMasterString, secondRunCraftMasterString);
        }

        [TestMethod]
        public void TestSingleInstanceConsumablesCanBeAssignedMultipleTimes()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestConsumablesOnly\";
            _opts.SeedText = "SeedText-SingleInstanceConsumableFindableTwice";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            const string singleInstanceConsumableName = "onlycraftable_consumable_1";
            int count = CountItemsWithNamesContaining(singleInstanceConsumableName, outputFiles.QuestList);
            count += CountItemsWithNamesContaining(singleInstanceConsumableName, outputFiles.DropList);
            Assert.IsTrue(count > 1);
        }

        [TestMethod]
        public void TestUniqueConsumablesAreNeverPlacedInCraftList()
        {
            // Assign
            _opts.InputPath = @"file-resources\ComponentTest\TestConsumablesOnly\";
            _opts.SeedText = "SeedText-UniqueConsumableNeverInCraftList";
            var gameFileReader = new GameFileService(_opts.InputPath);

            // Act
            Program.RunMain(_opts);
            GameFiles outputFiles = gameFileReader.ReadAllFiles(FolderPathOutput);

            //Assert
            int countUniqueConsumables = CountAllItemsWithNamesContaining("unique_consumable", outputFiles);
            Assert.AreEqual(0, countUniqueConsumables); 
        }

        private int CountAllItemsWithNamesContaining(string itemNamePart, GameFiles outputFiles)
        {
            int uniqueItemCount = CountItemsWithNamesContaining(itemNamePart, outputFiles.DropList);
            uniqueItemCount += CountItemsWithNamesContaining(itemNamePart, outputFiles.CraftList);
            uniqueItemCount += CountItemsWithNamesContaining(itemNamePart, outputFiles.QuestList);
            return uniqueItemCount;
        }

        private int CountItemsWithNamesContaining(string itemNamePart, IEnumerable<IItemEntry> itemEntryList)
        {
            return itemEntryList.Count(entry =>
                entry.IsEntryValid() && entry.GetItemName().StartsWith(itemNamePart));
        }

        private int GetResultingCraftListItemCount(IEnumerable<IItemEntry> itemEntryList)
        {
            return itemEntryList.Count(entry => !Constants.EntryInfoNone.Equals(entry.GetItemName()));
        }

        private static void AssertInputAndOutputItemTypesMatch<T>(List<T> inputItemList, List<T> outputItemList) where T : IItemEntry
        {
            int numberOfAddedEntriesFound = 0;
            for (int i = 0; i < inputItemList.Count; i++)
            {
                IItemEntry inputEntry = inputItemList[i];
                if (inputEntry.IsEntryValid())
                {
                    IItemEntry outputEntry = outputItemList[i];
                    if (outputEntry is CraftItemEntry && Constants.EntryInfoNone.Equals(outputEntry.GetItemName()))
                    {
                        int nextNewItemIndex = inputItemList.Count + numberOfAddedEntriesFound;
                        outputEntry = outputItemList[nextNewItemIndex];
                        numberOfAddedEntriesFound++;
                        Assert.AreEqual(inputEntry.GetItemType(), outputEntry.GetItemType());
                    }
                    else
                    {
                        Assert.AreEqual(inputEntry.GetItemType(), inputEntry.GetItemType());
                    }
                }
            }
        }

        private static void AssertInvalidEntriesAreEqual<T>(List<T> inputItemList, List<T> outputItemList) where T : IItemEntry
        {
            for (int i = 0; i < inputItemList.Count; i++)
            {
                if (!inputItemList[i].IsEntryValid())
                {
                    Assert.AreEqual(inputItemList[i], outputItemList[i]);
                }
            }
        }

    }
}