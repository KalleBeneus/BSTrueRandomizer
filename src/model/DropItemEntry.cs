using System;
using BSTrueRandomizer.Exceptions;

namespace BSTrueRandomizer.model
{
    public class DropItemEntry : IItemEntry
    {
        public DropItemEntry(string key, DropItemValues value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public DropItemValues Value { get; }

        public string GetItemName()
        {
            return GetFirstDropItemName();
        }

        public string GetItemType()
        {
            return Value.ItemType;
        }

        public string GetItemReferenceType()
        {
            if (Value.ItemReferenceType == null)
            {
                throw new IllegalStateException($"Item reference type has not been correctly set for {GetType().Name} with key '{Key}'");
            }
            return Value.ItemReferenceType;
        }

        public void UpdateItemType(string itemType)
        {
            Value.ItemReferenceType = itemType;
        }

        public bool IsEntryValid()
        {
            return !Key.ToLower().EndsWith("_shard") && !Constants.ItemTypeNone.Equals(GetItemType());
        }

        public bool IsEntryBreakableWall()
        {
            return Key.ToLower().StartsWith("wall_");
        }

        private string GetFirstDropItemName()
        {
            if (Constants.ItemTypeCoin.Equals(Value.ItemType))
            {
                return "Coin";
            }
            if (!Constants.EntryInfoNone.Equals(Value.CommonItemId))
            {
                return Value.CommonItemId;
            }
            if (!Constants.EntryInfoNone.Equals(Value.RareItemId))
            {
                return Value.RareItemId;
            }
            if (!Constants.EntryInfoNone.Equals(Value.CommonIngredientId))
            {
                return Value.CommonIngredientId;
            }
            if (!Constants.EntryInfoNone.Equals(Value.RareIngredientId))
            {
                return Value.RareIngredientId;
            }

            return Constants.EntryInfoNone;
        }

        protected bool Equals(DropItemEntry other)
        {
            return Key == other.Key && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DropItemEntry) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }
    }
}