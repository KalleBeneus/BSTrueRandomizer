using System.Collections.Generic;

namespace BSTrueRandomizer.model.composite
{
    public class GameFiles
    {
        public List<CraftItemEntry> CraftList { get; }
        public List<DropItemEntry> DropList { get; }
        public List<QuestItemEntry> QuestList { get; }

        internal GameFiles(List<CraftItemEntry> craftList, List<DropItemEntry> dropList, List<QuestItemEntry> questList)
        {
            CraftList = craftList;
            DropList = dropList;
            QuestList = questList;
        }
    }
}
