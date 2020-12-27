using System.Collections.Generic;
using System.Linq;
using BSTrueRandomizer.config;
using BSTrueRandomizer.model;
using BSTrueRandomizer.service;

namespace BSTrueRandomizer.mod
{
    /**
     * This class enables changing the item type of chests and breakables. These changes will only be reflected in the game when playing
     * on randomizer setting 'Item: Retain Type'. All chests with the same type value will then be shuffled between themselves.
     */
    internal class DropTypeRandomizerMod
    {
        private const string ItemTypeNonKey = Constants.ItemTypeWeapon;

        private readonly Dictionary<int, string> _fixedKeyLocations = new Dictionary<int, string>
        {
            {19, "VillageKeyBox"},
            {21, "PhotoEvent"},
            {22, "CertificationboardEvent"},
            {24, "Swordsman"},
            {183, "Treasurebox_SAN024"},
            {216, "Treasurebox_TWR019"},
            {217, "Treasurebox_TWR019"},
            {269, "Treasurebox_KNG021"},
            {386, "Treasurebox_ARC006"}
        };

        private readonly ItemRandomizerService _randomizerService;
        private readonly Options _options;

        public DropTypeRandomizerMod(ItemRandomizerService randomizerService, Options options)
        {
            _randomizerService = randomizerService;
            _options = options;
        }

        public void SetAllItemLocationsToSameType(List<DropItemEntry> gameDataToModify)
        {
            foreach (DropItemEntry entry in gameDataToModify.Where(IsEntryRandomizable))
            {
                entry.Value.ItemType = ItemTypeNonKey;
            }
        }

        private bool IsEntryRandomizable(DropItemEntry entry)
        {
            // Due to mistakes in the original game files, these types should be kept as-is to only allow them to shuffle between themselves.
            // This avoids problems with important items being shuffled out of the game
            string itemType = entry.GetItemType();
            return entry.IsEntryValid()
                   && !Constants.ItemTypeCraftingMaterials.Equals(itemType)
                   && !Constants.ItemTypeConsumable.Equals(itemType)
                   && !Constants.ItemTypeNone.Equals(itemType);
        }

        /**
         * Select a number of random chests to be potential key item locations. Chests that are found by defeating certain bosses are fixed
         * key item locations. For the player, this effectively makes key items be findable anywhere in the game, but chests protected by
         * bosses will have a higher chance of containing a key item. Key items will never be found in breakable walls.
         */
        public void SetRandomKeyItemLocations(List<DropItemEntry> gameDataToModify)
        {
            List<DropItemEntry> randomizableEntries = gameDataToModify.Where(IsKeyItemLocationCandidate).ToList();
            ICollection<DropItemEntry> entriesToSetAsKey = _randomizerService.GetRandomEntriesFromList(randomizableEntries, _options.NumberOfKeyLocations);

            SetKeyTypeForRandomAndFixedLocations(gameDataToModify, entriesToSetAsKey);
        }

        private bool IsKeyItemLocationCandidate(DropItemEntry entry)
        {
            return IsEntryRandomizable(entry) && !entry.IsEntryBreakableWall() && !IsFixedKeyItemLocation(entry);
        }

        private bool IsFixedKeyItemLocation(DropItemEntry itemEntry)
        {
            return _fixedKeyLocations.ContainsKey(itemEntry.Value.Id) && itemEntry.Key.Equals(_fixedKeyLocations[itemEntry.Value.Id]);
        }

        private void SetKeyTypeForRandomAndFixedLocations(IEnumerable<DropItemEntry> gameDataToModify, ICollection<DropItemEntry> entriesToSetAsKey)
        {
            foreach (DropItemEntry entry in gameDataToModify)
            {
                if (IsFixedKeyItemLocation(entry) || entriesToSetAsKey.Contains(entry))
                {
                    entry.Value.ItemType = Constants.ItemTypeKey;
                }
            }
        }
    }
}