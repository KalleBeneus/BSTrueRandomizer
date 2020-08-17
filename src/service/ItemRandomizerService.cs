using System;
using System.Collections.Generic;
using BSTrueRandomizer.Exceptions;
using BSTrueRandomizer.model.random;

namespace BSTrueRandomizer.service
{
    internal class ItemRandomizerService
    {
        private readonly Random _random;
        private RandomizableStore _itemSource;

        public ItemRandomizerService(Random random)
        {
            _random = random;
        }

        public void LoadItems(RandomizableStore itemSource)
        {
            _itemSource = itemSource;
        }

        public string GetRandomItemOfType(string itemType)
        {
            int itemCount = _itemSource.ItemCountByType(itemType);
            if (itemCount == 0)
            {
                throw new RandomizationException(
                    "No more items available for randomization. There may be a mismatch of available items and slots to randomize for");
            }

            int randomItemIndex = _random.Next(itemCount);
            RandomizableEntry item = _itemSource.PopSingleItemOccurrenceAtIndex(itemType, randomItemIndex);

            return item.ItemName;
        }

        public void RemoveItemFromRandomization(string itemType, string itemName)
        {
            if (itemType != null && itemName != null && _itemSource.ContainsItem(itemType, itemName))
            {
                _itemSource.RemoveItemByType(itemType, itemName);
            }
        }

        public bool IsItemOfTypeAvailable(string itemType, string itemName)
        {
            return _itemSource.ContainsItem(itemName, itemType);
        }

        public Dictionary<string, Dictionary<string, int>> CreateSnapshotOfAvailableItems()
        {
            throw new NotImplementedException();
        }

        public ICollection<int> GetRandomOpenSlotIndexesForRemainingItems(string itemType, int numberOfOpenSlots)
        {
            int itemCount = _itemSource.ItemCountByType(itemType);
            if (numberOfOpenSlots < itemCount)
            {
                throw new ArgumentException(
                    $"Number of open slots provided ({numberOfOpenSlots}) is less than number of items to randomize ({itemCount})");
            }

            SortedSet<int> openSlotIndexes = new SortedSet<int>();
            while (openSlotIndexes.Count < itemCount)
            {
                openSlotIndexes.Add(_random.Next(numberOfOpenSlots));
            }

            return openSlotIndexes;
        }

        public int GetRemainingNumberOfUniqueItemsByType(string itemType)
        {
            throw new NotImplementedException();
        }
    }
}