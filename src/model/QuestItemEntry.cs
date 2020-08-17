
using System;
using BSTrueRandomizer.Exceptions;

namespace BSTrueRandomizer.model
{
    public class QuestItemEntry : IItemEntry
    {
        public QuestItemEntry(string key, QuestItemValues value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public QuestItemValues Value { get; }

        public string GetItemName()
        {
            return Value.RewardItem01;
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
            return !"benjamin".Equals(Value.ClientId.ToLower());
        }

        public bool IsGourmandQuest()
        {
            return "susie".Equals(Value.ClientId.ToLower());
        }

        protected bool Equals(QuestItemEntry other)
        {
            return Key == other.Key && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((QuestItemEntry) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Value);
        }
    }
}
