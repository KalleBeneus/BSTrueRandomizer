using System;
using System.Collections.Generic;
using System.Text;

namespace BSTrueRandomizer.model
{
    class MasterItem
    {
        public string Key { get; }
        public MasterItemValues Value { get; }

        public MasterItem(string key, MasterItemValues value)
        {
            Key = key;
            Value = value;
        }
    }
}
