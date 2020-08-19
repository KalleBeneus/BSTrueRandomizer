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
            var randomizableItemStore = new RandomizableStore();

            AddAllItemsToRandomize(gameFilesToModify.DropList, randomizableItemStore);
            AddAllItemsToRandomize(gameFilesToModify.QuestList, randomizableItemStore);
            AddAllGourmandQuestFoods(gameFilesToModify.QuestList, randomizableItemStore);
            AddAllCraftableItemsToRandomize(gameFilesToModify.CraftList, randomizableItemStore);

            RandomizeDropItems(gameFilesToModify.DropList, randomizableItemStore);
            RandomizeQuestItems(gameFilesToModify.QuestList, randomizableItemStore);
            RandomizeCraftItems(gameFilesToModify.CraftList, randomizableItemStore);
        }

        private void AddAllItemsToRandomize(IEnumerable<IItemEntry> itemList, RandomizableStore storeToAddTo)
        {
            itemList.Where(IsEntryRandomizableForInput)
                .ToList()
                .ForEach(entry => storeToAddTo.AddItem(entry.GetItemName(), entry.GetItemType()));
        }

        private bool IsEntryRandomizableForInput(IItemEntry entry)
        {
            string itemType = entry.GetItemType();

            return entry.IsEntryValid() &&
                   !"EItemType::CraftingMats".Equals(itemType) &&
                   !"EItemType::Key".Equals(itemType) &&
                   !"EItemType::Upgrade".Equals(itemType) &&
                   !"EItemType::None".Equals(itemType) &&
                   !"EItemType::Coin".Equals(itemType) &&
                   !"EItemType::Recipe".Equals(itemType) &&
                   !"EItemType::Food".Equals(itemType) &&
                   !"EItemType::Consumable".Equals(itemType);
        }

        private static void AddAllGourmandQuestFoods(List<QuestItemEntry> questList, RandomizableStore storeToAddTo)
        {
            questList.Where(entry => entry.IsGourmandQuest())
                .ToList()
                .ForEach(entry => storeToAddTo.AddItem(entry.Value.Item01, Constants.ItemTypeFood));
        }

        private void AddAllCraftableItemsToRandomize(IEnumerable<IItemEntry> craftList, RandomizableStore storeToAddTo)
        {
            craftList.Where(IsEntryRandomizableForCraftInput)
                .ToList()
                .ForEach(entry => storeToAddTo.AddCraftableItem(entry.GetItemName(), entry.GetItemType()));
        }

        private bool IsEntryRandomizableForCraftInput(IItemEntry entry)
        {
            string itemType = entry.GetItemType();

            return entry.IsEntryValid() &&
                   !"EItemType::CraftingMats".Equals(itemType) &&
                   !"EItemType::Key".Equals(itemType) &&
                   !"EItemType::Upgrade".Equals(itemType) &&
                   !"EItemType::None".Equals(itemType) &&
                   !"EItemType::Coin".Equals(itemType) &&
                   !"EItemType::Food".Equals(itemType) &&
                   !"EItemType::Recipe".Equals(itemType);
        }

        private void RandomizeDropItems(List<DropItemEntry> dropList, RandomizableStore availableItems)
        {
            foreach (DropItemEntry dropEntry in dropList)
            {
                if (IsEntryRandomizableForOutput(dropEntry))
                {
                    string randomItemName = GetRandomItem(availableItems, dropEntry.GetItemType());
                    SetNewItemForDrop(dropEntry, randomItemName);
                }
            }
        }

        private bool IsEntryRandomizableForOutput(IItemEntry entry)
        {
            string itemType = entry.GetItemType();

            return entry.IsEntryValid() &&
                   !"EItemType::CraftingMats".Equals(itemType) &&
                   !"EItemType::Key".Equals(itemType) &&
                   !"EItemType::Upgrade".Equals(itemType) &&
                   !"EItemType::None".Equals(itemType) &&
                   !"EItemType::Coin".Equals(itemType) &&
                   !"EItemType::Recipe".Equals(itemType);
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

        private void RandomizeQuestItems(List<QuestItemEntry> questList, RandomizableStore availableItems)
        {
            foreach (QuestItemEntry questEntry in questList)
            {
                if (IsEntryRandomizableForOutput(questEntry))
                {
                    questEntry.Value.RewardItem01 = GetRandomItem(availableItems, questEntry.GetItemType());
                    questEntry.Value.RewardNum01 = 1;
                }
            }
        }

        private void RandomizeCraftItems(List<CraftItemEntry> craftList, RandomizableStore availableItems)
        {
            Dictionary<string, IEnumerable<string>> itemsToReplaceByType = FindAllItemsToReplace(availableItems);
            var newCraftEntries = new List<CraftItemEntry>();

            foreach (CraftItemEntry entry in craftList.Where(IsCraftEntryRandomizableForOutput))
            {
                IEnumerable<string> itemsToReplace = itemsToReplaceByType[entry.GetItemType()];
                if (itemsToReplace.Contains(entry.GetItemName()))
                {
                    string randomItemName = GetRandomNonCraftableItemName(availableItems, entry.GetItemType());
                    newCraftEntries.Add(CreateReplacementCraftEntry(entry, randomItemName));
                }
            }

            craftList.AddRange(newCraftEntries);
        }

        private Dictionary<string, IEnumerable<string>> FindAllItemsToReplace(RandomizableStore availableItems)
        {
            var itemsToReplaceDict = new Dictionary<string, IEnumerable<string>>();

            foreach (string itemType in availableItems.AvailableItemTypes().Where(IsItemTypeRandomizableForCraftOutput))
            {
                List<string> alreadyAssignedCraftItems = availableItems.GetAllUnavailableCraftableItemNames(itemType);
                int numberOfItemsLeftToAssign = availableItems.AvailableNonCraftableItemCountByType(itemType);
                itemsToReplaceDict[itemType] = _randomizerService.GetRandomEntriesFromList(alreadyAssignedCraftItems, numberOfItemsLeftToAssign);
            }

            return itemsToReplaceDict;
        }

        private bool IsCraftEntryRandomizableForOutput(CraftItemEntry craftEntry)
        {
            string itemType = craftEntry.GetItemType();

            return craftEntry.IsEntryValid() &&
                   IsItemTypeRandomizableForCraftOutput(itemType) &&
                   !"8bitcoin".Equals(craftEntry.Value.Ingredient1Id.ToLower()) &&
                   !"32bitcoin".Equals(craftEntry.Value.Ingredient2Id.ToLower());
        }

        private bool IsItemTypeRandomizableForCraftOutput(string itemType)
        {
            return !"EItemType::CraftingMats".Equals(itemType) &&
                   !"EItemType::Key".Equals(itemType) &&
                   !"EItemType::Upgrade".Equals(itemType) &&
                   !"EItemType::None".Equals(itemType) &&
                   !"EItemType::Coin".Equals(itemType) &&
                   !"EItemType::Consumable".Equals(itemType) &&
                   !"EItemType::Food".Equals(itemType) &&
                   !"EItemType::Recipe".Equals(itemType);
        }

        private string GetRandomNonCraftableItemName(RandomizableStore availableItems, string itemType)
        {
            int availableItemCount = availableItems.AvailableNonCraftableItemCountByType(itemType);
            int randomIndex = _randomizerService.GetRandomItemIndex(availableItemCount);
            string randomItemName = availableItems.TakeAllNonCraftableItem(itemType, randomIndex);
            return randomItemName;
        }

        private CraftItemEntry CreateReplacementCraftEntry(CraftItemEntry craftItem, string randomItemName)
        {
            craftItem.Value.CraftItemId = Constants.EntryInfoNone;
            CraftItemValues newCraftItemValues = craftItem.Value.Copy();
            newCraftItemValues.CraftItemId = randomItemName;
            return new CraftItemEntry(randomItemName, newCraftItemValues);
        }
    }
}