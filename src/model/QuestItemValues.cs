using Newtonsoft.Json;

namespace BSTrueRandomizer.model
{
    public class QuestItemValues
    {
        public string QuestType { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Caption { get; set; }
        public string ClientId { get; set; }
        public string Place { get; set; }
        public bool IsReAcceptable { get; set; }
        public string NeedQuestId { get; set; }
        public string NeedAreaId { get; set; }
        public string NeedItemId { get; set; }
        public string NeedBossId { get; set; }
        public string NeedDlc { get; set; }
        public string StartItemId { get; set; }
        public string Item01 { get; set; }
        public int ItemNum01 { get; set; }
        public string Item02 { get; set; }
        public int ItemNum02 { get; set; }
        public string Item03 { get; set; }
        public int ItemNum03 { get; set; }
        public bool EraseItem { get; set; }
        public bool NeedComplete { get; set; }
        public string Enemy01 { get; set; }
        public int EnemyNum01 { get; set; }
        public int Experience { get; set; }
        public string RewardItem01 { get; set; }
        public int RewardNum01 { get; set; }
        public string RewardOtherwise { get; set; }
        public string AcceptEventId { get; set; }
        public string CompletedEventId { get; set; }
        public string AcceptDialogueId { get; set; }
        public string CancelDialogueId { get; set; }
        public bool IsReject { get; set; }
        public string UnlockArchives { get; set; }
        public string EnemySpawnLocations { get; set; }

        [JsonIgnore]
        public string ItemReferenceType { get; set; }

        protected bool Equals(QuestItemValues other)
        {
            return QuestType == other.QuestType && 
                   Title == other.Title && 
                   Name == other.Name && 
                   Description == other.Description && 
                   Caption == other.Caption && 
                   ClientId == other.ClientId && 
                   Place == other.Place && 
                   IsReAcceptable == other.IsReAcceptable && 
                   NeedQuestId == other.NeedQuestId && 
                   NeedAreaId == other.NeedAreaId && 
                   NeedItemId == other.NeedItemId && 
                   NeedBossId == other.NeedBossId && 
                   NeedDlc == other.NeedDlc && 
                   StartItemId == other.StartItemId && 
                   Item01 == other.Item01 && 
                   ItemNum01 == other.ItemNum01 && 
                   Item02 == other.Item02 &&
                   ItemNum02 == other.ItemNum02 &&
                   Item03 == other.Item03 &&
                   ItemNum03 == other.ItemNum03 &&
                   EraseItem == other.EraseItem &&
                   NeedComplete == other.NeedComplete &&
                   Enemy01 == other.Enemy01 &&
                   EnemyNum01 == other.EnemyNum01 &&
                   Experience == other.Experience &&
                   RewardItem01 == other.RewardItem01 &&
                   RewardNum01 == other.RewardNum01 &&
                   RewardOtherwise == other.RewardOtherwise &&
                   AcceptEventId == other.AcceptEventId &&
                   CompletedEventId == other.CompletedEventId &&
                   AcceptDialogueId == other.AcceptDialogueId &&
                   CancelDialogueId == other.CancelDialogueId &&
                   IsReject == other.IsReject &&
                   UnlockArchives == other.UnlockArchives &&
                   EnemySpawnLocations == other.EnemySpawnLocations;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((QuestItemValues) obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
