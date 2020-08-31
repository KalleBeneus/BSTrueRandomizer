
using Newtonsoft.Json;

namespace BSTrueRandomizer.model
{
    public class DropItemValues
    {
        public int Id{ get; set; }
        public string ShardId{ get; set; }
        public double ShardRate{ get; set; }
        public string RareItemId{ get; set; }
        public int RareItemQuantity{ get; set; }
        public double RareItemRate{ get; set; }
        public string CommonItemId{ get; set; }
        public int CommonItemQuantity{ get; set; }
        public double CommonRate{ get; set; }
        public string RareIngredientId{ get; set; }
        public int RareIngredientQuantity{ get; set; }
        public double RareIngredientRate{ get; set; }
        public string CommonIngredientId{ get; set; }
        public int CommonIngredientQuantity{ get; set; }
        public double CommonIngredientRate{ get; set; }
        public string CoinType{ get; set; }
        public int CoinOverride{ get; set; }
        public double CoinRate{ get; set; }
        public bool AreaChangeTreasureFlag{ get; set; }
        public string ItemType{ get; set; }

        [JsonIgnore]
        public string ItemReferenceType { get; set; }

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

        public override bool Equals(object obj)
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
