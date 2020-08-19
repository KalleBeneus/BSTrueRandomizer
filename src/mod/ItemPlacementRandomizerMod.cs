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
            _randomizerService = randomizerService;
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

            RandomizeDropItems(dropList, availableItems);
            RandomizeQuestItems(questList, availableItems);


            Dictionary<string, int> currentSlotNumberByType = InitializeCurrentSlotCounters(availableItems);
            Dictionary<string, ICollection<int>> slotRandomizationIndexesByType = FindAllOpenSlotsForRandomization(availableItems);
            List<CraftItemEntry> newCraftEntries = new List<CraftItemEntry>();

            foreach (CraftItemEntry entry in craftList)
            {
                if (IsCraftEntryRandomizable(entry))
                {
                    string itemType = entry.GetItemType();
                    if (!availableItems.IsItemAvailable(entry.GetItemName(), itemType) &&
                        slotRandomizationIndexesByType[itemType].Count() != 0 &&
                        currentSlotNumberByType[itemType]++ == slotRandomizationIndexesByType[itemType].First())
                    {
                        slotRandomizationIndexesByType[itemType].Remove(slotRandomizationIndexesByType[itemType].First());
                        string randomItemName = GetRandomNonCraftableItemName(availableItems, itemType);
                        ReplaceCraftItem(newCraftEntries, entry, randomItemName);
                    }
                }
            }
            craftList.AddRange(newCraftEntries);
        }

        private static Dictionary<string, int> InitializeCurrentSlotCounters(RandomizableStore availableItems)
        {
            var currentSlotNumberByType = new Dictionary<string, int>();
            foreach (string itemType in availableItems.AvailableItemTypes())
            {
                currentSlotNumberByType[itemType] = 0;
            }

            return currentSlotNumberByType;
        }

        private Dictionary<string, ICollection<int>> FindAllOpenSlotsForRandomization(RandomizableStore availableItems)
        {
            var slotRandomizationIndexesByType = new Dictionary<string, ICollection<int>>();
            foreach (string itemType in availableItems.AvailableItemTypes())
            {
                if (IsItemTypeRandomizableForCraftOutput(itemType))
                {
                    int numberOfOpenItemSlots = availableItems.UnavailableCraftableItemCountByType(itemType);
                    int numberOfItemsToAssign = availableItems.AvailableNonCraftableItemCountByType(itemType);
                    slotRandomizationIndexesByType[itemType] = _randomizerService.GetRandomOpenSlotIndexes(numberOfOpenItemSlots, numberOfItemsToAssign);
                }
            }

            return slotRandomizationIndexesByType;
        }

        private static void ReplaceCraftItem(List<CraftItemEntry> newCraftEntries, CraftItemEntry craftItem, string randomItemName)
        {
            craftItem.Value.CraftItemId = Constants.EntryInfoNone;
            CraftItemValues newCraftItemValues = craftItem.Value.Copy();
            newCraftItemValues.CraftItemId = randomItemName;
            newCraftEntries.Add(new CraftItemEntry(randomItemName, newCraftItemValues));
        }

        private void RandomizeQuestItems(List<QuestItemEntry> questList, RandomizableStore availableItems)
        {
            foreach (QuestItemEntry questEntry in questList)
            {
                if (questEntry.IsEntryValid() && IsItemTypeRandomizableForOutput(questEntry.GetItemType()))
                {
                    questEntry.Value.RewardItem01 = GetRandomItem(availableItems, questEntry.GetItemType());
                    questEntry.Value.RewardNum01 = 1;
                }
            }
        }

        private void RandomizeDropItems(List<DropItemEntry> dropList, RandomizableStore availableItems)
        {
            foreach (DropItemEntry dropEntry in dropList)
            {
                if (dropEntry.IsEntryValid() && IsItemTypeRandomizableForOutput(dropEntry.GetItemType()))
                {
                    string randomItemName = GetRandomItem(availableItems, dropEntry.GetItemType());

                    SetNewItemForDrop(dropEntry, randomItemName);
                }
            }
        }

        private static void SetNewItemForDrop(DropItemEntry dropEntry, string randomItemName)
        {
            dropEntry.Value.RareItemId = randomItemName;
            dropEntry.Value.RareItemQuantity = 1;
            dropEntry.Value.RareItemRate = 100.0;

            dropEntry.Value.CommonItemId = Constants.EntryInfoNone;
            dropEntry.Value.CommonItemQuantity = 0;
            dropEntry.Value.CommonRate = 0.0;
            dropEntry.Value.CommonIngredientId = Constants.EntryInfoNone;
            dropEntry.Value.CommonIngredientQuantity = 0;
            dropEntry.Value.CommonIngredientRate = 0.0;
            dropEntry.Value.RareIngredientId = Constants.EntryInfoNone;
            dropEntry.Value.RareIngredientQuantity = 0;
            dropEntry.Value.RareIngredientRate = 0.0;
        }

        private string GetRandomItem(RandomizableStore availableItems, string itemType)
        {
            int availableItemCount = availableItems.AvailableItemCountByType(itemType);
            int randomIndex = _randomizerService.GetRandomItemIndex(availableItemCount);
            if (Constants.ItemTypeConsumable.Equals(itemType))
            {
                const int decrementAmount = 0;
                return availableItems.TakeNumberOfItem(itemType, randomIndex, decrementAmount);
            }

            return availableItems.TakeSingleItem(itemType, randomIndex);
        }

        private string GetRandomNonCraftableItemName(RandomizableStore availableItems, string itemType)
        {
            int availableItemCount = availableItems.AvailableNonCraftableItemCountByType(itemType);
            int randomIndex = _randomizerService.GetRandomItemIndex(availableItemCount);
            string randomItemName = availableItems.TakeAllNonCraftableItem(itemType, randomIndex);
            return randomItemName;
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
                .ForEach(entry => storeToAddTo.AddItem(entry.Value.Item01, Constants.ItemTypeFood));
        }

        private void AddAllFindableItems(IEnumerable<IItemEntry> itemList, RandomizableStore storeToAddTo)
        {
            itemList.Where(entry => entry.IsEntryValid() && IsItemTypeRandomizable(entry.GetItemType()))
                .ToList()
                .ForEach(entry => storeToAddTo.AddItem(entry.GetItemName(), entry.GetItemType()));
        }

        private bool IsItemTypeRandomizable(string itemType)
        {
            if ("EItemType::CraftingMats".Equals(itemType)
                || "EItemType::Key".Equals(itemType)
                || "EItemType::Upgrade".Equals(itemType)
                || "EItemType::None".Equals(itemType)
                || "EItemType::Coin".Equals(itemType)
                || "EItemType::Recipe".Equals(itemType)
                || "EItemType::Food".Equals(itemType)
                || "EItemType::Consumable".Equals(itemType))
                return false;
            return true;
        }

        private bool IsItemTypeRandomizableForOutput(string itemType)
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

        private bool IsCraftEntryRandomizable(CraftItemEntry craftEntry)
        {
            string itemType = craftEntry.GetItemType();

            return craftEntry.IsEntryValid() &&
                   IsItemTypeRandomizableForCraftOutput(itemType) &&
                   !"8bitcoin".Equals(craftEntry.Value.Ingredient1Id.ToLower()) &&
                   !"32bitcoin".Equals(craftEntry.Value.Ingredient2Id.ToLower());
        }

        private bool IsItemTypeRandomizableForCraftOutput(string itemType)
        {
            return
                !"EItemType::CraftingMats".Equals(itemType) &&
                !"EItemType::Key".Equals(itemType) &&
                !"EItemType::Upgrade".Equals(itemType) &&
                !"EItemType::None".Equals(itemType) &&
                !"EItemType::Coin".Equals(itemType) &&
                !"EItemType::Consumable".Equals(itemType) &&
                !"EItemType::Food".Equals(itemType) &&
                !"EItemType::Recipe".Equals(itemType);

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

    }
}