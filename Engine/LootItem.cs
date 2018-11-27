using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LootItem
    {
        public Item Details { get; set; }
        public int Number { get; set; }

        public LootItem(Item details, int quantity = 1)
        {
            Details = details;
            Number = quantity;
        }

        public override string ToString()
        {
            return Details.Name;
        }
    }
}
