namespace BSTrueRandomizer.model.random
{
    internal class RandomizableEntry
    {
        public string ItemName { get; }
        public string ItemType { get; }

        public int OccurrenceCount { get; private set; }

        public bool IsCraftable { get; set; }

        public RandomizableEntry(string itemName, string itemType, bool isCraftable)
        {
            ItemName = itemName;
            ItemType = itemType;
            IsCraftable = isCraftable;
            OccurrenceCount = 1;
        }

        public void IncrementOccurrence(int number = 1)
        {
            OccurrenceCount += number;
        }

        public void DecrementOccurrence(int number = 1)
        {
            OccurrenceCount -= number;
        }

        public void ZeroOccurrence()
        {
            OccurrenceCount = 0;
        }

        protected bool Equals(RandomizableEntry other)
        {
            return ItemName == other.ItemName;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RandomizableEntry) obj);
        }

        public override int GetHashCode()
        {
            return (ItemName != null ? ItemName.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return
                $"{nameof(ItemName)}: {ItemName}, {nameof(ItemType)}: {ItemType}, {nameof(OccurrenceCount)}: {OccurrenceCount}, {nameof(IsCraftable)}: {IsCraftable}";
        }
    }
}