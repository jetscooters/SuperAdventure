using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace Engine
{
    public class Player : LivingCreature
    {
        private int _gold;
        private int _exp;
        private Location _currentLocation;
        private Monster _currentMonster;

        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }
        public int ExperiencePoints
        {
            get { return _exp; }
            private set
            {
                _exp = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            }
        }
        public int Level
        {
            get { return ((ExperiencePoints / 100) + 1); }
        }
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }
        public Weapon CurrentWeapon { get; set; }
        public Monster CurrentMonster
        {
            get { return _currentMonster; }
            set
            {
                _currentMonster = value;
                OnPropertyChanged("CurrentMonster");
            }
        }
        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }
        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
        }
        public BindingList<InventoryItem> Inventory { get; set; }
        public BindingList<PlayerQuest> Quests { get; set; }

        public event EventHandler<MessageEventArgs> OnMessage;

        public Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.Inventory.Add(new InventoryItem(World.ItemByName("Rusty sword"), 1));
            player.CurrentLocation = World.LocationByName("Home");

            return player;
        }

        public void MoveNorth()
        {
            Location targ = CurrentLocation.ToNorth;

            if (targ != null)
            {
                MoveTo(targ);
            }
        }

        public void MoveEast()
        {
            Location targ = CurrentLocation.ToEast;

            if (targ != null)
            {
                MoveTo(targ);
            }
        }

        public void MoveSouth()
        {
            Location targ = CurrentLocation.ToSouth;

            if (targ != null)
            {
                MoveTo(targ);
            }
        }

        public void MoveWest()
        {
            Location targ = CurrentLocation.ToWest;

            if (targ != null)
            {
                MoveTo(targ);
            }
        }

        public void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (!HasRequiredItemToEnterThisLocation(newLocation))
            {
                RaiseMessage("You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location.", true);
                return;
            }

            // Update the player's current location
            CurrentLocation = newLocation;

            // Completely heal the player
            CurrentHitPoints = MaximumHitPoints;

            // Does the location have a monster?
            Monster mon = newLocation.GetMonster();
            if (mon != null)
            {
                RaiseMessage("You see a " + mon.Name + ".", true);
                CurrentMonster = World.NewMonster(World.MonsterByName(mon.Name));
            }
            else
            {
                CurrentMonster = mon;
            }

            // Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = CompletedThisQuest(newLocation.QuestAvailableHere);

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        // The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // Display message
                            RaiseMessage("You complete the '" + newLocation.QuestAvailableHere.Name + "' quest.", false);

                            // Remove quest items from inventory
                            RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            // Give quest rewards
                            RaiseMessage("You receive: ", false);
                            RaiseMessage(newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points.", false);
                            RaiseMessage(newLocation.QuestAvailableHere.RewardGold.ToString() + " gold.", false);
                            foreach (InventoryItem i in newLocation.QuestAvailableHere.RewardItems)
                            {
                                RaiseMessage(i.Quantity + " " + i.ItemName + ".", false);
                            }
                            RaiseMessage("", false);
                            
                            AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
                            Gold += newLocation.QuestAvailableHere.RewardGold;

                            // Add the reward item to the player's inventory                            
                            foreach (InventoryItem i in newLocation.QuestAvailableHere.RewardItems)
                            {
                                AddItemToInventory(i.Details, i.Quantity);
                            }

                            // Mark the quest as completed
                            // Find the quest in the player's quest list
                            MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    RaiseMessage("You receive the " + newLocation.QuestAvailableHere.Name + " quest.", false);
                    RaiseMessage(newLocation.QuestAvailableHere.Description, false);
                    RaiseMessage("To complete it, return with:", false);
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.Name, false);
                        }
                        else
                        {
                            RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.NamePlural, false);
                        }
                    }
                    RaiseMessage("", false);

                    // Add the quest to the player's quest list
                    Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }
        }

        public void UseWeapon(Weapon weapon)
        {
            //Determine damage to monster
            int damageToMonster = RandomNumberGenerator.IntBetween(weapon.MinimumDamage, weapon.MaximumDamage);

            //Apply damage to monster
            CurrentMonster.CurrentHitPoints -= damageToMonster;

            //Display message
            RaiseMessage("You hit the " + CurrentMonster.Name + " for " + damageToMonster.ToString() + " points.", true);

            //Check if monster is dead
            if (CurrentMonster.CurrentHitPoints <= 0)
            {
                //monster is dead
                RaiseMessage("You defeated the " + CurrentMonster.Name + ".", false);

                //Give player experience points for killing the monster
                AddExperiencePoints(CurrentMonster.RewardExperiencePoints);
                RaiseMessage("You recieve " + CurrentMonster.RewardExperiencePoints.ToString() + " experience.", false);

                //Give player gold for killing the monster
                Gold += CurrentMonster.RewardGold;
                RaiseMessage("You recieve " + CurrentMonster.RewardGold.ToString() + " gold.", false);

                //Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                //Add items to the lootedItems list, comparing a random number to a drop percentage
                LootItem thingLooted = World.MonsterByID(CurrentMonster.ID).WeightedLootTable.GetRandomItemFromWeightedTable();
                InventoryItem itemToAdd = new InventoryItem(thingLooted.Details, thingLooted.Number);
                lootedItems.Add(itemToAdd);

                //Add the looted items to the player's inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    AddItemToInventory(inventoryItem.Details, inventoryItem.Quantity);

                    RaiseMessage("You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.ItemName + ".", true);
                }

                //Move player to current location to heal and create a new monster
                MoveTo(CurrentLocation);
            }
            else
            {
                //Monster is still alive
                MonsterTurn(CurrentMonster);           
            }
        }

        public void UsePotion(HealingPotion potion)
        {
            CurrentHitPoints += potion.AmountToHeal;

            if (CurrentHitPoints > MaximumHitPoints)
            {
                CurrentHitPoints = MaximumHitPoints;
            }

            RemoveItemFromInventory(potion);

            RaiseMessage("You drink the " + potion.Name + " and heal " + potion.AmountToHeal + " HP.", true);

            MonsterTurn(CurrentMonster);
        }

        public void MonsterTurn(Monster monster)
        {
            //Determine monster damage
            int damageToPlayer = RandomNumberGenerator.IntBetween(0, CurrentMonster.MaximumDamage);

            //Display message
            RaiseMessage("The " + CurrentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage.", true);

            //subtract damage from player
            CurrentHitPoints -= damageToPlayer;

            if (CurrentHitPoints <= 0)
            {
                //Display message
                RaiseMessage("The " + CurrentMonster.Name + " killed you.", true);

                //Move player Home
                MoveTo(World.LocationByName("Home"));
            }
        }

        public void AddExperiencePoints(int num)
        {
            ExperiencePoints += num;
            MaximumHitPoints = Level * 10;
        }

        public bool HasRequiredItemToEnterThisLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                // There is no required item for this location, so return "true"
                return true;
            }

            // See if the player has the required item in their inventory
            return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
        }

        public bool HasThisQuest(Quest quest)
        {
            return Quests.Any(q => q.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    return playerQuest.IsCompleted;
                }
            }

            return false;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            // See if the player has all the items needed to complete the quest here
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }
            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                // Subtract the quantity from the player's inventory that was needed to complete the quest
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);

                if (item != null)
                {
                    RemoveItemFromInventory(item.Details, qci.Quantity);
                }
            }
        }

        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

            if (item == null)
            {
                // They didn't have the item, so add it to their inventory
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                // They have the item in their inventory, so increase the quantity
                item.Quantity += quantity;
            }
            RaiseInventoryChangedEvent(itemToAdd);
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);

            if (item != null)
            {
                item.Quantity -= quantity;

                if (item.Quantity < 0)
                {
                    item.Quantity = 0;
                }

                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }

                RaiseInventoryChangedEvent(itemToRemove);
            }
        }

        public void MarkQuestCompleted(Quest quest)
        {
            // Find the quest in the player's quest list
            PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == quest.ID);

            if (playerQuest != null)
            {
                playerQuest.IsCompleted = true;
            }
        }

        private void RaiseMessage(string message, bool newLine)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new MessageEventArgs(message, newLine));
            }
        }

        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }

            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }
        
        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            // Create the top-level XML node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            // Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);
            
            SaveLoad.AddNode(playerData, stats, "CurrentHitPoints", CurrentHitPoints);
            SaveLoad.AddNode(playerData, stats, "MaximumHitPoints", MaximumHitPoints);
            SaveLoad.AddNode(playerData, stats, "Gold", Gold);
            SaveLoad.AddNode(playerData, stats, "ExperiencePoints", ExperiencePoints);
            SaveLoad.AddNode(playerData, stats, "CurrentLocation", CurrentLocation.ID);

            if (CurrentWeapon != null)
            {
                SaveLoad.AddNode(playerData, stats, "CurrentWeapon", CurrentWeapon.ID);
            }

            // Create the "InventoryItems" child node to hold each InventoryItem node
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            // Create an "InventoryItem" node for each item in the player's inventory
            foreach (InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");

                SaveLoad.AddAttribute(playerData, inventoryItem, "ID", item.ItemID);
                SaveLoad.AddAttribute(playerData, inventoryItem, "Quantity", item.Quantity);

                inventoryItems.AppendChild(inventoryItem);
            }

            // Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            // Create a "PlayerQuest" node for each quest the player has acquired
            foreach (PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                SaveLoad.AddAttribute(playerData, playerQuest, "ID", quest.Details.ID);
                SaveLoad.AddAttribute(playerData, playerQuest, "IsCompleted", quest.IsCompleted);

                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml; // The XML document, as a string, so we can save the data to disk
        }
    }
}
