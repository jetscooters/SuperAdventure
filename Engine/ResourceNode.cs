using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ResourceNode
    {
        public string Name;
        public Item Resource;
        public ToolType ToolRequired;

        public ResourceNode(string name, Item resourceHere, ToolType toolRequired = ToolType.NONE)
        {
            Name = name;
            Resource = resourceHere;
            ToolRequired = toolRequired;
        }
    }
}
