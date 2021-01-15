using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BSTrueRandomizer.model
{
    [DataContract]
    public class DropItemValues
    {
        [DataMember(IsRequired = true)] public int Id { get; set; }
        [DataMember(IsRequired = true)] public string DropSpecialFlags { get; set; } = default!;
        [DataMember(IsRequired = true)] public string ShardId { get; set; } = default!;
        [DataMember(IsRequired = true)] public double ShardRate { get; set; }
        [DataMember(IsRequired = true)] public string RareItemId { get; set; } = default!;
        [DataMember(IsRequired = true)] public int RareItemQuantity { get; set; }
        [DataMember(IsRequired = true)] public double RareItemRate { get; set; }
        [DataMember(IsRequired = true)] public string CommonItemId { get; set; } = default!;
        [DataMember(IsRequired = true)] public int CommonItemQuantity { get; set; }
        [DataMember(IsRequired = true)] public double CommonRate { get; set; }
        [DataMember(IsRequired = true)] public string RareIngredientId { get; set; } = default!;
        [DataMember(IsRequired = true)] public int RareIngredientQuantity { get; set; }
        [DataMember(IsRequired = true)] public double RareIngredientRate { get; set; }
        [DataMember(IsRequired = true)] public string CommonIngredientId { get; set; } = default!;
        [DataMember(IsRequired = true)] public int CommonIngredientQuantity { get; set; }
        [DataMember(IsRequired = true)] public double CommonIngredientRate { get; set; }
        [DataMember(IsRequired = true)] public string CoinType { get; set; } = default!;
        [DataMember(IsRequired = true)] public int CoinOverride { get; set; }
        [DataMember(IsRequired = true)] public double CoinRate { get; set; }
        [DataMember(IsRequired = true)] public bool AreaChangeTreasureFlag { get; set; }
        [DataMember(IsRequired = true)] public string ItemType { get; set; } = default!;

        [JsonIgnore] public string? ItemReferenceType { get; set; }

        protected bool Equals(DropItemValues other)
        {
            return Id == other.Id &&
                   ShardId == other.ShardId &&
                   ShardRate.Equals(other.ShardRate) &&
                   RareItemId == other.RareItemId &&
                   RareItemQuantity == other.RareItemQuantity &&
                   RareItemRate.Equals(other.RareItemRate) &&
                   CommonItemId == other.CommonItemId &&
                   CommonItemQuantity == other.CommonItemQuantity &&
                   CommonRate.Equals(other.CommonRate) &&
                   RareIngredientId == other.RareIngredientId &&
                   RareIngredientQuantity == other.RareIngredientQuantity &&
                   RareIngredientRate.Equals(other.RareIngredientRate) &&
                   CommonIngredientId == other.CommonIngredientId &&
                   CommonIngredientQuantity == other.CommonIngredientQuantity &&
                   CommonIngredientRate.Equals(other.CommonIngredientRate) &&
                   CoinType == other.CoinType &&
                   CoinOverride == other.CoinOverride &&
                   CoinRate.Equals(other.CoinRate) &&
                   AreaChangeTreasureFlag == other.AreaChangeTreasureFlag &&
                   ItemType == other.ItemType;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DropItemValues) obj);
        }

        public override int GetHashCode()
        {
            return 2;
        }
    }
}