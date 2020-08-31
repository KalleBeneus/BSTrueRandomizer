using System;
using System.Collections.Generic;
using System.Linq;
using BSTrueRandomizer.config;
using BSTrueRandomizer.model;

namespace BSTrueRandomizer.service
{
    internal class ItemTypeConverter
    {
        private static Dictionary<string, string> _masterItemTypes;

        private static readonly Dictionary<string, string> TypeConversionMap = new Dictionary<string, string>
        {
            {"ECarriedCatalog::Potion", Constants.ItemTypeConsumable},
            {"ECarriedCatalog::Food", Constants.ItemTypeFood},
            {"ECarriedCatalog::FoodStuff", Constants.ItemTypeCraftingMaterials},
            {"ECarriedCatalog::Ingredient", Constants.ItemTypeCraftingMaterials},
            {"ECarriedCatalog::Seed", Constants.ItemTypeCraftingMaterials},
            {"ECarriedCatalog::Weapon", Constants.ItemTypeWeapon},
            {"ECarriedCatalog::Bullet", Constants.ItemTypeBullet},
            {"ECarriedCatalog::Head", Constants.ItemTypeHead},
            {"ECarriedCatalog::Body", Constants.ItemTypeBody},
            {"ECarriedCatalog::Accessory1", Constants.ItemTypeAccessory},
            {"ECarriedCatalog::Muffler", Constants.ItemTypeScarf},
            {"ECarriedCatalog::Key", Constants.ItemTypeKey},
            {"ECarriedCatalog::Book", Constants.ItemTypeNone},
            {"ECarriedCatalog::TriggerShard", Constants.ItemTypeNone},
            {"ECarriedCatalog::EffectiveShard", Constants.ItemTypeNone},
            {"ECarriedCatalog::DirectionalShard", Constants.ItemTypeNone},
            {"ECarriedCatalog::EnchantShard", Constants.ItemTypeNone},
            {"ECarriedCatalog::Skill", Constants.ItemTypeNone},
            {"ECarriedCatalog::FamiliarShard", Constants.ItemTypeNone}
        };

        public ItemTypeConverter(IEnumerable<MasterItem> itemMasterList)
        {
            _masterItemTypes = itemMasterList.ToDictionary(entry => entry.Key, entry => entry.Value.ItemType);
            if (_masterItemTypes.Count == 0)
            {
                throw new ArgumentException($"'{Constants.FileNameItemMaster}' file did not contain any item data");
            }
        }

        public string FindAndConvertItemTypeFromName(string itemName)
        {
            if (itemName == null || Constants.ItemNameNone.Equals(itemName) || !_masterItemTypes.ContainsKey(itemName))
            {
                return Constants.ItemTypeNone;
            }

            if ("Coin".Equals(itemName) || "Money".Equals(itemName))
            {
                return Constants.ItemTypeCoin;
            }

            if ("MaxHPUP".Equals(itemName) || "MaxMPUP".Equals(itemName) || "MaxBulletUP".Equals(itemName))
            {
                return Constants.ItemTypeUpgrade;
            }

            string masterItemType = _masterItemTypes[itemName];
            return TypeConversionMap[masterItemType];
        }
    }
}