namespace BSTrueRandomizer.model
{
    public class MasterItem
    {
        public MasterItem(string key, MasterItemValues value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public MasterItemValues Value { get; }
    }
}