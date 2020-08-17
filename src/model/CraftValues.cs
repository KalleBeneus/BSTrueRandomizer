using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BSTrueRandomizer.model
{
    public class CraftItemValues
    {
        public string Type { get; set; }
        public string CraftItemId { get; set; }
        public int CraftValue { get; set; }
        public int RankMin { get; set; }
        public int RankMax { get; set; }
        public string Ingredient1Id { get; set; }
        public int Ingredient1Total { get; set; }
        public string Ingredient2Id { get; set; }
        public int Ingredient2Total { get; set; }
        public string Ingredient3Id { get; set; }
        public int Ingredient3Total { get; set; }
        public string Ingredient4Id { get; set; }
        public int Ingredient4Total { get; set; }
        public string OpenKeyRecipeId { get; set; }
        public string OpenCondition { get; set; }
        public int OpenParameter { get; set; }
        public string FirstTimeBonusSpecialEffectId { get; set; }
        public List<string> FirstBonusType { get; set; }
        public List<string> FirstBonusValue { get; set; }
        public int Alkhahest { get; set; }

        [JsonIgnore]
        public string ItemReferenceType { get; set; }

        public CraftItemValues Copy()
        {
            return (CraftItemValues) MemberwiseClone();
        }

        protected bool Equals(CraftItemValues other)
        {
            return Type == other.Type &&
                   CraftItemId == other.CraftItemId &&
                   CraftValue == other.CraftValue &&
                   RankMin == other.RankMin &&
                   RankMax == other.RankMax &&
                   Ingredient1Id == other.Ingredient1Id &&
                   Ingredient1Total == other.Ingredient1Total &&
                   Ingredient2Id == other.Ingredient2Id &&
                   Ingredient2Total == other.Ingredient2Total &&
                   Ingredient3Id == other.Ingredient3Id &&
                   Ingredient3Total == other.Ingredient3Total &&
                   Ingredient4Id == other.Ingredient4Id &&
                   Ingredient4Total == other.Ingredient4Total &&
                   OpenKeyRecipeId == other.OpenKeyRecipeId &&
                   OpenCondition == other.OpenCondition &&
                   OpenParameter == other.OpenParameter &&
                   FirstTimeBonusSpecialEffectId == other.FirstTimeBonusSpecialEffectId &&
                   FirstBonusType.SequenceEqual(other.FirstBonusType) &&
                   FirstBonusValue.SequenceEqual(other.FirstBonusValue) &&
                   Alkhahest == other.Alkhahest;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CraftItemValues) obj);
        }

        public override int GetHashCode()
        {
            return 3;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(CraftItemId)}: {CraftItemId}, {nameof(CraftValue)}: {CraftValue}, {nameof(RankMin)}: {RankMin}, " +
                   $"{nameof(RankMax)}: {RankMax}, {nameof(Ingredient1Id)}: {Ingredient1Id}, {nameof(Ingredient1Total)}: {Ingredient1Total}, {nameof(Ingredient2Id)}: " +
                   $"{Ingredient2Id}, {nameof(Ingredient2Total)}: {Ingredient2Total}, {nameof(Ingredient3Id)}: {Ingredient3Id}, {nameof(Ingredient3Total)}: " +
                   $"{Ingredient3Total}, {nameof(Ingredient4Id)}: {Ingredient4Id}, {nameof(Ingredient4Total)}: {Ingredient4Total}, {nameof(OpenKeyRecipeId)}: " +
                   $"{OpenKeyRecipeId}, {nameof(OpenCondition)}: {OpenCondition}, {nameof(OpenParameter)}: {OpenParameter}, {nameof(FirstTimeBonusSpecialEffectId)}: " +
                   $"{FirstTimeBonusSpecialEffectId}, {nameof(FirstBonusType)}: {FirstBonusType}, {nameof(FirstBonusValue)}: {FirstBonusValue}, {nameof(Alkhahest)}: {Alkhahest}";
        }
    }

}
