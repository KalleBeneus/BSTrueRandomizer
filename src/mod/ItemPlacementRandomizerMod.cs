using System;
using System.Collections.Generic;
using System.Linq;
using BSTrueRandomizer.model;
using BSTrueRandomizer.model.composite;
using BSTrueRandomizer.model.random;
using BSTrueRandomizer.service;

namespace BSTrueRandomizer.mod
{
    internal class ItemPlacementRandomizerMod
    {
        private readonly ItemRandomizerService _randomizerService;

        public ItemPlacementRandomizerMod(ItemRandomizerService randomizerService)
        {
            this._randomizerService = randomizerService;
        }

        public void RandomizeItems(GameFiles gameFilesToModify)
        {
            List<CraftItemEntry> craftList = gameFilesToModify.CraftList;
            List<DropItemEntry> dropList = gameFilesToModify.DropList;
            List<QuestItemEntry> questList = gameFilesToModify.QuestList;

            var availableItems = new RandomizableStore();

            AddAllFindableItems(dropList, availableItems);
            AddAllFindableItems(questList, availableItems);
            AddAllGourmandQuestFoods(questList, availableItems);
            AddAllCraftableItems(craftList, availableItems);


            _randomizerService.LoadItems(availableItems);
            foreach (DropItemEntry dropItem in dropList)
                if (!dropItem.Key.ToLower().EndsWith("_shard") && IsItemTypeRandomizableOutput(dropItem.Value.ItemType))
                {
                    string itemType = dropItem.Value.ItemType;

                    dropItem.Value.CommonItemId = Constants.EntryInfoNone;
                    dropItem.Value.CommonItemQuantity = 0;
                    dropItem.Value.CommonRate = 0.0;
                    dropItem.Value.CommonIngredientId = Constants.EntryInfoNone;
                    dropItem.Value.CommonIngredientQuantity = 0;
                    dropItem.Value.CommonIngredientRate = 0.0;
                    dropItem.Value.RareIngredientId = Constants.EntryInfoNone;
                    dropItem.Value.RareIngredientQuantity = 0;
                    dropItem.Value.RareIngredientRate = 0.0;

                    dropItem.Value.RareItemId = _randomizerService.GetRandomItemOfType(itemType);
                    dropItem.Value.RareItemQuantity = 1;
                    dropItem.Value.RareItemRate = 100.0;
                }

            foreach (QuestItemEntry questEntry in questList)
            {
                string itemType = questEntry.GetItemType();
                if (questEntry.IsEntryValid() && IsItemTypeRandomizableOutput(itemType))
                {
                    questEntry.Value.RewardItem01 = _randomizerService.GetRandomItemOfType(itemType);
                    questEntry.Value.RewardNum01 = 1;
                }
            }

            Dictionary<string, Dictionary<string, int>> availableItemsForCrafting = _randomizerService.CreateSnapshotOfAvailableItems();
            RemoveNormallyCraftableItemsFromRandomizer(craftItemsByType, _randomizerService);
            var slotRandomizationIndexesByType = new Dictionary<string, ICollection<int>>();
            var currentSlotNumberByType = new Dictionary<string, int>();
            var newItemsToAddToCraftList = new List<CraftItemEntry>();
            foreach (var craftItem in craftList)
            {
                string itemType = craftItem.GetItemType();
                if (craftItem.IsEntryValid() && IsItemTypeRandomizableForCraftOutput(itemType) &&
                    !"8bitcoin".Equals(craftItem.Value.Ingredient1Id.ToLower()) && !"32bitcoin".Equals(craftItem.Value.Ingredient2Id.ToLower()))
                {
                    if (!slotRandomizationIndexesByType.ContainsKey(itemType))
                    {
                        int numberOfOpenSlotsForRemainingItems = craftItemsByType[itemType].Count -
                                                                 (availableItemsForCrafting[itemType].Count -
                                                                  _randomizerService.GetRemainingNumberOfUniqueItemsByType(itemType));
                        slotRandomizationIndexesByType[itemType] =
                            _randomizerService.GetRandomOpenSlotIndexesForRemainingItems(itemType, numberOfOpenSlotsForRemainingItems);
                        currentSlotNumberByType[itemType] = 0;
                    }

                    if (!availableItemsForCrafting[itemType].ContainsKey(craftItem.Value.CraftItemId) &&
                        slotRandomizationIndexesByType[itemType].Count() != 0 &&
                        currentSlotNumberByType[itemType]++ == slotRandomizationIndexesByType[itemType].First())
                    {
                        slotRandomizationIndexesByType[itemType].Remove(slotRandomizationIndexesByType[itemType].First());
                        string randomItemName = _randomizerService.GetRandomItemOfType(itemType);
                        craftItem.Value.CraftItemId = Constants.EntryInfoNone;
                        CraftItemValues newCraftItemValues = craftItem.Value.Copy();
                        newCraftItemValues.CraftItemId = randomItemName;
                        newItemsToAddToCraftList.Add(new CraftItemEntry(randomItemName, newCraftItemValues));
                        _randomizerService.RemoveItemFromRandomization(itemType, randomItemName);
                    }
                }
            }

            craftList.AddRange(newItemsToAddToCraftList);
        }

        private void AddAllCraftableItems(IEnumerable<IItemEntry> craftList, RandomizableStore storeToAddTo)
        {
            craftList.Where(entry => entry.IsEntryValid() && IsItemTypeRandomizableForCraftInput(entry.GetItemType()))
                .ToList()
                .ForEach(entry => storeToAddTo.AddCraftableItem(entry.GetItemName(), entry.GetItemType()));
        }

        private static void AddAllGourmandQuestFoods(List<QuestItemEntry> questList, RandomizableStore storeToAddTo)
        {
            questList.Where(entry => entry.IsGourmandQuest())
                .ToList()
                .ForEach(entry => storeToAddTo.AddItem(entry.Value.NeedItemId, Constants.ItemTypeFood));
        }

        private void AddAllFindableItems(IEnumerable<IItemEntry> dropList, RandomizableStore storeToAddTo)
        {
            dropList.Where(entry => entry.IsEntryValid() && IsItemTypeRandomizable(entry.GetItemType()))
                .ToList()
                .ForEach(entry => storeToAddTo.AddItem(entry.GetItemName(), entry.GetItemType()));
        }

        private static void RemoveNormallyCraftableItemsFromRandomizer(Dictionary<string, Dictionary<string, int>> craftItemsByType,
            ItemRandomizerService randomizerService)
        {
            foreach (string itemType in craftItemsByType.Keys)
            foreach (string itemName in craftItemsByType[itemType].Keys)
                randomizerService.RemoveItemFromRandomization(itemType, itemName);
        }

        private Dictionary<string, Dictionary<string, int>> MergeItemMaps(Dictionary<string, Dictionary<string, int>> firstDictByType,
            Dictionary<string, Dictionary<string, int>> secondDictByType, Func<int, int, int> applyOnCollision)
        {
            Dictionary<string, Dictionary<string, int>> resultDict =
                secondDictByType.ToDictionary(entry => entry.Key, entry => new Dictionary<string, int>(entry.Value));
            foreach (string itemType in firstDictByType.Keys)
                if (!resultDict.ContainsKey(itemType))
                {
                    resultDict.Add(itemType, firstDictByType[itemType]);
                }
                else
                {
                    Dictionary<string, int> targetItemDict = resultDict[itemType];
                    foreach (KeyValuePair<string, int> sourceItemEntry in firstDictByType[itemType])
                        if (targetItemDict.ContainsKey(sourceItemEntry.Key))
                            targetItemDict[sourceItemEntry.Key] =
                                applyOnCollision(sourceItemEntry.Value, targetItemDict[sourceItemEntry.Key]);
                        else
                            targetItemDict.Add(sourceItemEntry.Key, sourceItemEntry.Value);
                }

            return resultDict;
        }

        private Dictionary<string, Dictionary<string, int>> CreateItemMapByTypeForQuests(List<QuestItemEntry> questList)
        {
            var availableItemsByType = new Dictionary<string, Dictionary<string, int>>();
            availableItemsByType["EItemType::Food"] = new Dictionary<string, int>();

            foreach (var questEntry in questList)
            {
                if (questEntry.IsGourmandQuest())
                {
                    availableItemsByType["EItemType::Food"].Add(questEntry.Value.Item01, 1);
                }

                if (questEntry.IsEntryValid() && IsItemTypeRandomizable(questEntry.GetItemType()))
                {
                    string itemType = questEntry.GetItemType();
                    string itemToAdd = questEntry.Value.RewardItem01;
                    if (!availableItemsByType.ContainsKey(itemType))
                    {
                        var itemMap = new Dictionary<string, int> {{itemToAdd, 1}};
                        availableItemsByType.Add(itemType, itemMap);
                    }
                    else
                    {
                        Dictionary<string, int> itemMap = availableItemsByType[itemType];
                        itemMap[itemToAdd] = itemMap.ContainsKey(itemToAdd) ? ++itemMap[itemToAdd] : 1;
                    }
                }
            }

            return availableItemsByType;
        }

        private Dictionary<string, Dictionary<string, int>> CreateItemMapByTypeForCrafting(List<CraftItemEntry> craftList)
        {
            var availableItemsByType = new Dictionary<string, Dictionary<string, int>>();
            foreach (var craftEntry in craftList)
            {
                if (craftEntry.IsEntryValid() && IsItemTypeRandomizableForCraftInput(craftEntry.GetItemType()))
                {
                    string itemType = craftEntry.GetItemType();
                    string itemToAdd = craftEntry.Value.CraftItemId;
                    if (!availableItemsByType.ContainsKey(itemType)) availableItemsByType.Add(itemType, new Dictionary<string, int>());
                    availableItemsByType[itemType].Add(itemToAdd, 1);
                }
            }

            return availableItemsByType;
        }

        private Dictionary<string, Dictionary<string, int>> CreateItemMapByTypeForDrops(List<DropItemEntry> dropList)
        {
            var availableItemsByType = new Dictionary<string, Dictionary<string, int>>();
            foreach (DropItemEntry chestEntry in dropList)
                if (chestEntry.IsEntryValid() && IsItemTypeRandomizable(chestEntry.Value.ItemType))
                {
                    string itemType = chestEntry.Value.ItemType;
                    string itemToAdd = chestEntry.GetItemName();
                    if (!availableItemsByType.ContainsKey(itemType))
                    {
                        Dictionary<string, int> itemMap = new Dictionary<string, int> {{itemToAdd, 1}};
                        availableItemsByType.Add(itemType, itemMap);
                    }
                    else
                    {
                        Dictionary<string, int> itemMap = availableItemsByType[itemType];
                        itemMap[itemToAdd] = itemMap.ContainsKey(itemToAdd) ? ++itemMap[itemToAdd] : 1;
                    }
                }

            return availableItemsByType;
        }

        private bool IsItemTypeRandomizable(string itemType)
        {
            if ("EItemType::CraftingMats".Equals(itemType)
                || "EItemType::Key".Equals(itemType)
                || "EItemType::Upgrade".Equals(itemType)
                || "EItemType::None".Equals(itemType)
                || "EItemType::Coin".Equals(itemType)
                || "EItemType::Recipe".Equals(itemType)
                || "EItemType::Food".Equals(itemType))
                return false;
            return true;
        }

        private bool IsItemTypeRandomizableOutput(string itemType)
        {
            if ("EItemType::CraftingMats".Equals(itemType)
                || "EItemType::Key".Equals(itemType)
                || "EItemType::Upgrade".Equals(itemType)
                || "EItemType::None".Equals(itemType)
                || "EItemType::Coin".Equals(itemType)
                || "EItemType::Recipe".Equals(itemType))
                return false;
            return true;
        }

        private bool IsItemTypeRandomizableForCraftOutput(string itemType)
        {
            if ("EItemType::CraftingMats".Equals(itemType)
                || "EItemType::Key".Equals(itemType)
                || "EItemType::Upgrade".Equals(itemType)
                || "EItemType::None".Equals(itemType)
                || "EItemType::Coin".Equals(itemType)
                || "EItemType::Consumable".Equals(itemType)
                || "EItemType::Food".Equals(itemType)
                || "EItemType::Recipe".Equals(itemType))
                return false;
            return true;
        }

        private bool IsItemTypeRandomizableForCraftInput(string itemType)
        {
            if ("EItemType::CraftingMats".Equals(itemType)
                || "EItemType::Key".Equals(itemType)
                || "EItemType::Upgrade".Equals(itemType)
                || "EItemType::None".Equals(itemType)
                || "EItemType::Coin".Equals(itemType)
                || "EItemType::Food".Equals(itemType)
                || "EItemType::Recipe".Equals(itemType))
                return false;
            return true;
        }

        private static void PrintItemTypeDict(Dictionary<string, Dictionary<string, int>> availableItemsByType)
        {
            var lines = availableItemsByType.Select(kvp =>
                kvp.Key + ": " + string.Join(Environment.NewLine, kvp.Value.Select(subkvp => subkvp.Key + ": " + subkvp.Value)));
            Console.WriteLine(string.Join(Environment.NewLine, lines));
        }
    }
}