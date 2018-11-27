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
        public List<InventoryItem> RewardItems { get; set; }
        public List<QuestCompletionItem> QuestCompletionItems { get; set; }

        public Quest(string name, string description, int rewardExperiencePoints, int rewardGold)
        {
            ID = NextID;
            NextID++;

            Name = name;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            RewardItems = new List<InventoryItem>();
            QuestCompletionItems = new List<QuestCompletionItem>();

            Description = description + "\n" +
                rewardGold + " Gold.\n" +
                rewardExperiencePoints + " Experience.\n";
        }

        public void AddRewardItem(Item item, int quantity)
        {
            RewardItems.Add(new InventoryItem(item, quantity));

            Description += quantity + " ";
            Description += (quantity > 1 ? item.NamePlural : item.Name);
            Description += "\n";
        }
    }
}
