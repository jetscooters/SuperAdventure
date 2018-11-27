using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Item
    {
        public static int NextID = 0;
        public int ID { get; set; }
        public string Name { get; set; }
        public string NamePlural { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public Item(string name, string namePlural, string desc, int price)
        {
            if (World.ItemByName(name) == null)
            {
                ID = NextID;
                NextID++;
            }
            else
            {
                ID = World.MonsterByName(name).ID;
            }

            Name = name;
            NamePlural = namePlural;
            Price = price;
            Description = desc;
        }
    }
}
