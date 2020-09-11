using BSTrueRandomizer.config;
using BSTrueRandomizer.model.composite;

namespace BSTrueRandomizer.mod
{
    internal class ModManager
    {
        private readonly Options _opts;
        private readonly ItemPlacementRandomizerMod _itemPlacementRandomizerMod;
        private readonly DropTypeRandomizerMod _dropTypeRandomizerMod;

        public ModManager(Options opts, ItemPlacementRandomizerMod itemPlacementRandomizerMod, DropTypeRandomizerMod dropTypeRandomizerMod)
        {
            _dropTypeRandomizerMod = dropTypeRandomizerMod;
            _itemPlacementRandomizerMod = itemPlacementRandomizerMod;
            _opts = opts;
        }

        public void ApplyMods(GameFiles gameFiles)
        {
            _itemPlacementRandomizerMod.RandomizeItems(gameFiles);

            if (_opts.IsRandomizeType)
            {
                _dropTypeRandomizerMod.SetAllItemLocationsToSameType(gameFiles.DropList);
                _dropTypeRandomizerMod.SetRandomKeyItemLocations(gameFiles.DropList);
            }
        }
    }
}