namespace BSTrueRandomizer.model
{
    public interface IItemEntry
    {
        public string GetItemName();

        public string GetItemType();

        public void UpdateItemType(string itemType);

        public bool IsEntryValid();
    }
}
