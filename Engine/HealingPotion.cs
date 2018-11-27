using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HealingPotion : Item
    {
        public int AmountToHeal { get; set; }

        public HealingPotion(string name, string namePlural, int amountToHeal, string desc, int price) 
            : base(name, 
                  namePlural, 
                  desc 
                  + ("\nHeals " + amountToHeal + " points of health."), 
                  price)
        {
            AmountToHeal = amountToHeal;
        }
    }
}
