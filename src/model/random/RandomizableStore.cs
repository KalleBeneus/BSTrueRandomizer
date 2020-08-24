using System.Collections.Generic;
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

        public List<string> GetAllUnavailableCraftableItemNames(string itemType)
        {
            if (!_unavailableItems.ContainsKey(itemType))
            {
                return new List<string>();
            }
            return _unavailableItems[itemType]
                .Where(item => item.IsCraftable)
                .Select(item => item.ItemName)
                .ToList();
        }

        public IEnumerable<string> AvailableItemTypes()
        {
            return _availableItems.Keys;
        }

        public string TakeSingleItem(string itemType, int availableItemIndex)
        {
            return TakeNumberOfItem(itemType, availableItemIndex, 1);
        }

        public string TakeNumberOfItem(string itemType, int availableItemIndex, int numberToTake)
        {
            RandomizableEntry item = _availableItems[itemType][availableItemIndex];
            item.DecrementOccurrence(numberToTake);
            if (item.OccurrenceCount <= 0)
            {
                _availableItems[itemType].RemoveAt(availableItemIndex);
                AddNewItem(item, _unavailableItems);
            }

            return item.ItemName;
        }

        public string TakeAllNonCraftableItem(string itemType, int availableItemIndex)
        {
            RandomizableEntry uniqueItem = _availableItems[itemType]
                .Where(item => !item.IsCraftable)
                .ElementAt(availableItemIndex);

            uniqueItem.ZeroOccurrence();
            _availableItems[itemType].Remove(uniqueItem);
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