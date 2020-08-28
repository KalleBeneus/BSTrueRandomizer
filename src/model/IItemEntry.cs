namespace BSTrueRandomizer.model
{
    public interface IItemEntry
    {
        string Key { get; }

        string GetItemName();

        string GetItemType();

        void UpdateItemType(string itemType);

        bool IsEntryValid();
    }
}
