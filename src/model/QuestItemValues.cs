using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BSTrueRandomizer.model
{
    [DataContract]
    public class QuestItemValues
    {
        [DataMember(IsRequired = true)] public string QuestType { get; set; } = default!;
        [DataMember(IsRequired = true)] public string Title { get; set; } = default!;
        [DataMember(IsRequired = true)] public string Name { get; set; } = default!;
        [DataMember(IsRequired = true)] public string Description { get; set; } = default!;
        [DataMember(IsRequired = true)] public string Caption { get; set; } = default!;
        [DataMember(IsRequired = true)] public string ClientId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string Place { get; set; } = default!;
        [DataMember(IsRequired = true)] public bool IsReAcceptable { get; set; }
        [DataMember(IsRequired = true)] public string NeedQuestId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string NeedAreaId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string NeedItemId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string NeedBossId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string NeedDlc { get; set; } = default!;
        [DataMember(IsRequired = true)] public string StartItemId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string Item01 { get; set; } = default!;
        [DataMember(IsRequired = true)] public int ItemNum01 { get; set; }
        [DataMember(IsRequired = true)] public string Item02 { get; set; } = default!;
        [DataMember(IsRequired = true)] public int ItemNum02 { get; set; }
        [DataMember(IsRequired = true)] public string Item03 { get; set; } = default!;
        [DataMember(IsRequired = true)] public int ItemNum03 { get; set; }
        [DataMember(IsRequired = true)] public bool EraseItem { get; set; }
        [DataMember(IsRequired = true)] public bool NeedComplete { get; set; }
        [DataMember(IsRequired = true)] public string Enemy01 { get; set; } = default!;
        [DataMember(IsRequired = true)] public int EnemyNum01 { get; set; }
        [DataMember(IsRequired = true)] public int Experience { get; set; }
        [DataMember(IsRequired = true)] public string RewardItem01 { get; set; } = default!;
        [DataMember(IsRequired = true)] public int RewardNum01 { get; set; }
        [DataMember(IsRequired = true)] public string RewardOtherwise { get; set; } = default!;
        [DataMember(IsRequired = true)] public string AcceptEventId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string CompletedEventId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string AcceptDialogueId { get; set; } = default!;
        [DataMember(IsRequired = true)] public string CancelDialogueId { get; set; } = default!;
        [DataMember(IsRequired = true)] public bool IsReject { get; set; }
        [DataMember(IsRequired = true)] public string UnlockArchives { get; set; } = default!;
        [DataMember(IsRequired = true)] public string EnemySpawnLocations { get; set; } = default!;

        [JsonIgnore]
        public string? ItemReferenceType { get; set; }

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

        public override bool Equals(object? obj)
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