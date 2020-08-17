using System;
using System.Collections.Generic;
using System.Linq;
using BSTrueRandomizer.model;

namespace BSTrueRandomizer.modification
{
    class DropTypeRandomizerMod
    {
        private readonly Dictionary<string, string> _fixedKeyLocations = new Dictionary<string, string>() {
            { "VillageKeyBox", "EItemType::Key"},
            { "PhotoEvent", "EItemType::Key"},
            { "CertificationboardEvent", "EItemType::Key"},
            { "Swordsman", "EItemType::Key"},
            { "Treasurebox_SAN024", "EItemType::Weapon"},
            { "Treasurebox_TWR019", "EItemType::Key"},
            { "Treasurebox_KNG021", "EItemType::UniqueCraft"},
            { "Treasurebox_ARC006", "EItemType::Accessory"}
        };

        public void RandomizeTypesWithLimitedFixedKeyLocations(List<DropItemEntry> ItemListToModify)
        {
            int numberOfRandomKeyLocations = 15;
            int numberOfRandomizableEntries = CalculateNumberOfPossibleKeyItemSlots(ItemListToModify);
            ICollection<int> randomKeyEntryIndexes = GetRandomIndexesForNewKeyEntries(numberOfRandomizableEntries, numberOfRandomKeyLocations);

            RandomizeKeyItemEntries(randomKeyEntryIndexes, ItemListToModify);
        }


        private int CalculateNumberOfPossibleKeyItemSlots(List<DropItemEntry> dropList)
        {
            int numberOfRandomizableEntries = 0;
            foreach (DropItemEntry itemEntry in dropList)
            {
                if (itemEntry.IsEntryValid() && !itemEntry.IsEntryBreakableWall() && IsItemTypeRandomizable(itemEntry.Value.ItemType))
                {
                    numberOfRandomizableEntries++;
                }
            }
            numberOfRandomizableEntries -= _fixedKeyLocations.Count;
            return numberOfRandomizableEntries;
        }

        private bool IsItemTypeRandomizable(string itemType)
        {
            if ("EItemType::CraftingMats".Equals(itemType)
                || "EItemType::Consumable".Equals(itemType)
                || "EItemType::None".Equals(itemType))
            {
                return false;
            }
            return true;
        }

        private ICollection<int> GetRandomIndexesForNewKeyEntries(int numberOfOpenSlots, int numberOfDesiredSlots)
        {
            var random = new Random();

            if (numberOfOpenSlots < numberOfDesiredSlots)
            {
                throw new ArgumentException(
                    $"Number of open slots provided ({numberOfOpenSlots}) is less than desired number of slots ({numberOfDesiredSlots})");
            }
            var openSlotIndexes = new SortedSet<int>();
            while (openSlotIndexes.Count < numberOfDesiredSlots)
            {
                openSlotIndexes.Add(random.Next(numberOfOpenSlots));
            }
            return openSlotIndexes;
        }

        private void RandomizeKeyItemEntries(ICollection<int> randomKeyEntryIndexes, List<DropItemEntry> dropList)
        {
            int currentValidRandomizationEntryIndex = 0;
            foreach (DropItemEntry itemEntry in dropList)
            {
                if (itemEntry.IsEntryValid() && IsItemTypeRandomizable(itemEntry.Value.ItemType))
                {
                    if (_fixedKeyLocations.ContainsKey(itemEntry.Key) && itemEntry.Value.ItemType.Equals(_fixedKeyLocations[itemEntry.Key]))
                    {
                        itemEntry.Value.ItemType = Constants.ItemTypeKey;
                    }
                    else
                    {
                        if (randomKeyEntryIndexes.Count > 0 && randomKeyEntryIndexes.First() == currentValidRandomizationEntryIndex)
                        {
                            itemEntry.Value.ItemType = Constants.ItemTypeKey;
                            randomKeyEntryIndexes.Remove(currentValidRandomizationEntryIndex);
                        }
                        else
                        {
                            itemEntry.Value.ItemType = Constants.ItemTypeOther;
                        }
                        currentValidRandomizationEntryIndex++;
                    }
                }
            }
        }
    }
}
