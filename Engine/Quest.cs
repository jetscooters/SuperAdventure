using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest
    {
        public static int NextID = 0;
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }
        public List<Item> RewardItems { get; set; }
        public List<QuestCompletionItem> QuestCompletionItems { get; set; }

        public Quest(string name, string description, int rewardExperiencePoints, int rewardGold)
        {
            ID = NextID;
            NextID++;

            Name = name;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            RewardItems = new List<Item>();
            QuestCompletionItems = new List<QuestCompletionItem>();

            Description = description;
        }

        public void WriteRealDescription()
        {
            string items = "";
            foreach (Item i in RewardItems)
            {
                items += i.Name + "\n";
            }
            Description += "\n"
               + "Rewards: \n"
               + RewardExperiencePoints + " experience\n"
               + RewardGold + " gold\n"
               + items;
        }
    }
}
