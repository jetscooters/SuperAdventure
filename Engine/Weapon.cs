using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Weapon : Item
    {
        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public Weapon(string name, string namePlural, int minimumDamage, int maximumDamage, string desc, int price) : base(name, namePlural, desc + "\nDamage range: " + minimumDamage + " - " + maximumDamage, price)
        {
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
        }
    }
}
