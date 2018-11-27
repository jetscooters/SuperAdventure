using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Tool : Weapon
    {
        public ToolType Type;

        public bool IsToolType(ToolType type)
        {
            return Type == type;
        }

        public Tool(string name, string namePlural, int minDamage, int maxDamage, string desc, int price, ToolType type):base(name, namePlural, minDamage, maxDamage, desc, price)
        {
            Type = type;
        }
    }

    public enum ToolType
    {
        HAMMER = 0,
        AXE = 1,
        PICK = 2
    }
}
