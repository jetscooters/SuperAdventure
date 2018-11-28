using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static List<Quest> Quests = new List<Quest>();
        public static List<Location> Locations = new List<Location>();

        public const int UNSELLABLE_ITEM_PRICE = -1;

        static World()
        {
            //PopulateItems();
            //PopulateMonsters();
            //PopulateQuests();
            //PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon("Rusty sword", "Rusty swords", 0, 5, "This sword has seen better days.", 5));
            Items.Add(new Weapon("Broadsword", "Broadswords", 5, 9, "A blade worthy of a lord.", 7));
            Items.Add(new Item("Piece of fur", "Pieces of fur", "Fur from some kind of mammal.", 1));
            Items.Add(new Item("Rat tail", "Rat tails", "The severed tail of a rodent.", 1));
            Items.Add(new Item("Snake fang", "Snake fangs", "The teeth of a serpent.", 1));
            Items.Add(new Item("Snakeskin", "Snakeskins", "Skin from a snake.", 2));
            Items.Add(new Weapon("Club", "Clubs", 7, 10, "A heavy length of wood.", 8));
            Items.Add(new Tool("Felling axe", "Felling axes", 7, 10, "An axe used to cut trees.", 7, ToolType.AXE));
            Items.Add(new HealingPotion("Healing potion", "Healing potions", 5, "A simple healing tincture.", 3));
            Items.Add(new Item("Spider fang", "Spider fangs", "A fang from some giant arachnoid.", 1));
            Items.Add(new Item("Spider silk", "Spider silks", "They say the stuff is stronger than steel.", 1));
            Items.Add(new Item("Adventurer pass", "Adventurer passes", "Used to get past the gate gaurd.", UNSELLABLE_ITEM_PRICE));
            Items.Add(new Item("Wood", "Wood", "A piece of wood cut from a tree.", 1));
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster("Rat", 5, 3, 10, 3, 3);
            rat.WeightedLootTable.AddItem(new LootItem(ItemByName("Rat tail")), 10);
            rat.WeightedLootTable.AddItem(new LootItem(ItemByName("Piece of fur")), 30);

            Monster snake = new Monster("Snake", 5, 3, 10, 3, 3);
            snake.WeightedLootTable.AddItem(new LootItem(ItemByName("Snake fang"), 3), 10);
            snake.WeightedLootTable.AddItem(new LootItem(ItemByName("Snakeskin")), 10);

            Monster giantSpider = new Monster("Giant spider", 20, 5, 40, 10, 10);
            giantSpider.WeightedLootTable.AddItem(new LootItem(ItemByName("Spider fang"), 2), 3);
            giantSpider.WeightedLootTable.AddItem(new LootItem(ItemByName("Spider silk"), 3), 1);

            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(
                    "Clear the alchemist's garden",
                    "Kill rats in the alchemist's garden and bring back 3 rat tails.",
                    20, 10);

            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByName("Rat tail"), 3));
            clearAlchemistGarden.AddRewardItem(ItemByName("Healing potion"), 3);
            clearAlchemistGarden.AddRewardItem(ItemByName("Broadsword"), 1);

            Quest clearFarmersField =
                new Quest(
                    "Clear the farmer's field",
                    "Kill snakes in the farmer's field and bring back 3 snake fangs.",
                    20, 20);

            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByName("Snake fang"), 3));
            clearFarmersField.AddRewardItem(ItemByName("Adventurer pass"), 1);

            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
        }

        private static void PopulateLocations()
        {
            // Create each location
            Location home = new Location("Home", "Your house. You really need to clean up the place.");

            Location townSquare = new Location("Town square", "You see a fountain.");
                        
            Location alchemistHut = new Location("Alchemist's hut", "There are many strange plants on the shelves.");
            alchemistHut.QuestAvailableHere = QuestByName("Clear the alchemist's garden");

            Location alchemistsGarden = new Location("Alchemist's garden", "Many plants are growing here.");

            Location farmhouse = new Location("Farmhouse", "There is a small farmhouse, with a farmer in front.");
            farmhouse.QuestAvailableHere = QuestByName("Clear the farmer's field");

            Location farmersField = new Location("Farmer's field", "You see rows of vegetables growing here.");

            Location guardPost = new Location("Guard post", "There is a large, tough-looking guard here.", ItemByName("Adventurer pass"));

            Location bridge = new Location("Bridge", "A stone bridge crosses a wide river.");

            Location spiderField = new Location("Forest", "You see spider webs covering covering the trees in this forest.");

            Location smithy = new Location("Blacksmith", "The smell of iron fills the air.");

            // Link the locations together
            home.ToNorth = townSquare;

            townSquare.ToNorth = alchemistHut;
            townSquare.ToSouth = home;
            townSquare.ToEast = guardPost;
            townSquare.ToWest = farmhouse;

            farmhouse.ToEast = townSquare;
            farmhouse.ToWest = farmersField;
            farmhouse.ToSouth = smithy;

            smithy.ToNorth = farmhouse;

            farmersField.ToEast = farmhouse;

            alchemistHut.ToSouth = townSquare;
            alchemistHut.ToNorth = alchemistsGarden;

            alchemistsGarden.ToSouth = alchemistHut;

            guardPost.ToEast = bridge;
            guardPost.ToWest = townSquare;

            bridge.ToWest = guardPost;
            bridge.ToEast = spiderField;

            spiderField.ToWest = bridge;

            // Add vendors
            Vendor bob = new Vendor("Bob the Rat-Catcher");
            bob.AddItemToInventory(ItemByName("Piece of fur"), 5);
            bob.AddItemToInventory(ItemByName("Rat tail"), 3);
            townSquare.VendorWorkingHere = bob;

            Vendor alchemist = new Vendor("Alchemist");
            alchemist.AddItemToInventory(ItemByName("Healing potion"), 5);
            alchemistHut.VendorWorkingHere = alchemist;

            Vendor blacksmith = new Vendor("Blacksmith");
            blacksmith.AddItemToInventory(ItemByName("Broadsword"), 5);
            blacksmith.AddItemToInventory(ItemByName("Club"), 5);
            blacksmith.AddItemToInventory(ItemByName("Felling axe"), 3);
            smithy.VendorWorkingHere = blacksmith;

            // Add resources
            ResourceNode tree = new ResourceNode("Tree", ItemByName("Wood"), ToolType.AXE);
            spiderField.ResourceHere = tree;

            // Add monsters
            townSquare.MonstersHere.AddItem(MonsterByName("Rat"), 1);
            townSquare.MonstersHere.AddItem(null, 5);

            alchemistsGarden.MonstersHere.AddItem(MonsterByName("Rat"), 1);

            farmersField.MonstersHere.AddItem(MonsterByName("Snake"), 1);

            spiderField.MonstersHere.AddItem(MonsterByName("Giant spider"), 1);
            spiderField.MonstersHere.AddItem(MonsterByName("Snake"), 3);
            spiderField.MonstersHere.AddItem(MonsterByName("Rat"), 6);

            // Add the locations to the static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }

        public static Item ItemByID(int id)
        {
            return Items.SingleOrDefault(i => i.ID == id);
        }

        public static Item ItemByName(string name)
        {
            return Items.SingleOrDefault(i => i.Name.Equals(name));           
        }

        public static Monster MonsterByID(int id)
        {
            return Monsters.SingleOrDefault(m => m.ID == id);
        }

        public static Monster MonsterByName(string name)
        {
            return Monsters.SingleOrDefault(m => m.Name.Equals(name));
        }

        public static Monster NewMonster(Monster monster)
        {
            return new Monster(monster.Name, monster.MaximumDamage, monster.RewardExperiencePoints, monster.RewardGold, monster.CurrentHitPoints, monster.MaximumHitPoints);
        }

        public static Quest QuestByID(int id)
        {
            return Quests.SingleOrDefault(q => q.ID == id);
        }

        public static Quest QuestByName(string name)
        {
            return Quests.SingleOrDefault(q => q.Name.Equals(name));
        }

        public static Location LocationByID(int id)
        {
            return Locations.SingleOrDefault(l => l.ID == id);
        }

        public static Location LocationByName(string name)
        {
            return Locations.SingleOrDefault(l => l.Name.Equals(name));
        }

        public static void Initi()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

    }

    
}
