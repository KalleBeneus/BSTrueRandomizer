using System;
using BSTrueRandomizer.Exceptions;

namespace BSTrueRandomizer.model
{
    public class CraftItemEntry : IItemEntry
    {
        public CraftItemEntry(string key, CraftItemValues value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public CraftItemValues Value { get; }

        public string GetItemName()
        {
            return Value.CraftItemId;
        }

        public string GetItemType()
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
            return "ECraftType::Craft".Equals(Value.Type);
        }

        protected bool Equals(CraftItemEntry other)
        {
            return Key == other.Key && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CraftItemEntry) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}";
        }
    }
}
