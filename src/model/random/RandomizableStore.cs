﻿using System.Collections.Generic;
using System.Linq;

namespace BSTrueRandomizer.model.random
{
    internal class RandomizableStore
    {
        private readonly Dictionary<string, List<RandomizableEntry>> _availableItems;
        private readonly Dictionary<string, List<RandomizableEntry>> _unavailableItems;

        public RandomizableStore()
        {
            _availableItems = new Dictionary<string, List<RandomizableEntry>>();
            _unavailableItems = new Dictionary<string, List<RandomizableEntry>>();
        }

        public int AvailableItemCountByType(string itemType)
        {
            return _availableItems[itemType].Count; // TODO add contains checks for all methods taking itemType
        }

        public int AvailableNonCraftableItemCountByType(string itemType)
        {
            return _availableItems[itemType].Count(item => !item.IsCraftable);
        }

        public int UnavailableCraftableItemCountByType(string itemType)
        {
            if (!_unavailableItems.ContainsKey(itemType))
            {
                return 0;
            }
            return _unavailableItems[itemType].Count(item => item.IsCraftable);
        }

        public IEnumerable<string> AvailableItemTypes()
        {
            return _availableItems.Keys;
        }

        public string TakeAndDecrementItem(string itemType, int availableItemIndex)
        {
            RandomizableEntry item = _availableItems[itemType][availableItemIndex];
            item.DecrementOccurrence();
            if (item.OccurrenceCount <= 0)
            {
                _availableItems[itemType].RemoveAt(availableItemIndex);
                AddNewItem(item, _unavailableItems);
            }

            return item.ItemName;
        }

        public string TakeAndRemoveNonCraftableItem(string itemType, int availableItemIndex)
        {
            RandomizableEntry uniqueItem = _availableItems[itemType]
                .Where(item => !item.IsCraftable)
                .ElementAt(availableItemIndex);

            uniqueItem.ZeroOccurrence();
            _availableItems[itemType].RemoveAt(availableItemIndex);
            AddNewItem(uniqueItem, _unavailableItems);
            return uniqueItem.ItemName;
        }

        public void AddItem(string itemName, string itemType)
        {
            var randomizableItem = new RandomizableEntry(itemName, itemType, false);
            AddItem(randomizableItem);
        }

        public void AddCraftableItem(string itemName, string itemType)
        {
            var randomizableItem = new RandomizableEntry(itemName, itemType, true);
            AddItem(randomizableItem);
        }

        private void AddItem(RandomizableEntry itemToAdd)
        {
            if (!IsItemAvailable(itemToAdd.ItemName, itemToAdd.ItemType))
            {
                AddNewItem(itemToAdd, _availableItems);
            }
            else if (!itemToAdd.IsCraftable)
            {
                IncrementItemOccurrence(itemToAdd);
            }
            else
            {
                MarkItemCraftable(itemToAdd);
            }
        }

        private void AddNewItem(RandomizableEntry itemToAdd, Dictionary<string, List<RandomizableEntry>> targetDict)
        {
            string itemType = itemToAdd.ItemType;
            if (!targetDict.ContainsKey(itemType))
            {
                targetDict.Add(itemType, new List<RandomizableEntry>());
            }

            targetDict[itemType].Add(itemToAdd);
        }

        private void IncrementItemOccurrence(RandomizableEntry itemToAdd)
        {
            RandomizableEntry existingEntry = _availableItems[itemToAdd.ItemType].Find(entry => entry.Equals(itemToAdd));
            existingEntry?.IncrementOccurrence();
        }

        private void MarkItemCraftable(RandomizableEntry itemToAdd)
        {
            RandomizableEntry existingEntry = _availableItems[itemToAdd.ItemType].Find(entry => entry.Equals(itemToAdd));
            if (existingEntry != null)
            {
                existingEntry.IsCraftable = true;
            }
        }

        public bool IsItemAvailable(string itemName, string itemType)
        {
            return _availableItems.ContainsKey(itemType) && _availableItems[itemType].Exists(item => item.ItemName.Equals(itemName));
        }
    }
}