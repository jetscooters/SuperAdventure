using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Location
    {
        public static int NextID = 0;
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Item ItemRequiredToEnter { get; set; }
        public Quest QuestAvailableHere { get; set; }
        public WeightedTable<Monster> MonstersHere { get; set; }
        public Location ToNorth { get; set; }
        public Location ToEast { get; set; }
        public Location ToSouth { get; set; }
        public Location ToWest { get; set; }
        public Vendor VendorWorkingHere { get; set; }
        public ResourceNode ResourceHere { get; set; }

        public Monster GetMonster()
        {
            return MonstersHere.GetRandomItemFromWeightedTable();
        }

        public Location(string name, string description, Item key = null, Quest questHere = null)
        {
            ID = NextID;
            NextID++;

            Name = name;
            Description = description;
            ItemRequiredToEnter = key;
            QuestAvailableHere = questHere;
            MonstersHere = new WeightedTable<Monster>();
        }
    }
}
