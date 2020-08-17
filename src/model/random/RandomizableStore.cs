using System.Collections.Generic;

namespace BSTrueRandomizer.model.random
{
    internal class RandomizableStore
    {
        private readonly Dictionary<string, List<RandomizableEntry>> _itemDict;

        public RandomizableStore()
        {
            _itemDict = new Dictionary<string, List<RandomizableEntry>>();
        }

        public int ItemCountByType(string itemType)
        {
            return _itemDict[itemType].Count; // TODO add contains checks for all methods taking itemType
        }

        public RandomizableEntry PopSingleItemOccurrenceAtIndex(string itemType, int index)
        {
            RandomizableEntry item = _itemDict[itemType][index];
            if (item.OccurrenceCount == 1)
            {
                _itemDict[itemType].RemoveAt(index);
            }
            else
            {
                item.DecrementOccurrence();
            }
            return item;
        }

        public void RemoveItemByType(string itemType, string itemName)
        {
            var itemToRemove = new RandomizableEntry(itemType, itemName, false);
            _itemDict[itemType].Remove(itemToRemove);
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

        public void AddItem(RandomizableEntry itemToAdd)
        {
            if (!ContainsItem(itemToAdd.ItemName, itemToAdd.ItemType))
            {
                AddItemByType(itemToAdd);
            }
            else if (!itemToAdd.IsCraftable)
            {
                IncrementItemOccurrence(itemToAdd);
            }
        }

        private void AddItemByType(RandomizableEntry itemToAdd)
        {
            string itemType = itemToAdd.ItemType;
            if (!_itemDict.ContainsKey(itemType))
            {
                _itemDict.Add(itemType, new List<RandomizableEntry>());
            }

            _itemDict[itemType].Add(itemToAdd);
        }

        public bool ContainsItem(string itemName, string itemType)
        {
            return _itemDict.ContainsKey(itemType) && _itemDict[itemType].Exists(item => item.ItemName.Equals(itemName));
        }

        private void IncrementItemOccurrence(RandomizableEntry itemToAdd)
        {
            RandomizableEntry existingEntry = _itemDict[itemToAdd.ItemType].Find(entry => entry.Equals(itemToAdd));
            existingEntry?.IncrementOccurrence();
        }

    }
}