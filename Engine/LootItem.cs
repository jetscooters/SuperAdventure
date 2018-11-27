﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LootItem
    {
        public Item Details { get; set; }
        public bool IsDefaultItem { get; set; }

        public LootItem(Item details, bool isDefaultItem)
        {
            Details = details;
            IsDefaultItem = isDefaultItem;
        }

        public override string ToString()
        {
            return Details.Name;
        }
    }
}
