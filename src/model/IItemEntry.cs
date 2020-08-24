namespace BSTrueRandomizer.model
{
    public interface IItemEntry
    {
        string GetItemName();

        string GetItemType();

        void UpdateItemType(string itemType);

        bool IsEntryValid();
    }
}
