using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Monster : LivingCreature
    {
        public static int NextID = 0;
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }
        public WeightedTable<LootItem> WeightedLootTable { get; set; }

        public Monster(string name, int maxDamage, int rwdExp, int rwdGold, int curHP, int maxHP) : base(curHP, maxHP)
        {
            if (World.MonsterByName(name) == null)
            {
                ID = NextID;
                NextID++;
            }
            else
            {
                ID = World.MonsterByName(name).ID;
            }
            

            Name = name;
            MaximumDamage = maxDamage;
            RewardExperiencePoints = rwdExp;
            RewardGold = rwdGold;
            WeightedLootTable = new WeightedTable<LootItem>();
        }
    }
}
