﻿using System.ComponentModel;
using System.Linq;

namespace Engine
{
    public class Vendor : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public BindingList<InventoryItem> Inventory { get; set; }

        public Vendor(string name)
        {
            Name = name;
            Inventory = new BindingList<InventoryItem>();
        }

        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

            if (item == null)
            {
                //they didn't have the item, so add it to their inventory
                Inventory.Add(new InventoryItem(itemToAdd, quantity));

            }
            else
            {
                //They have the item, so increase the quantity
                item.Quantity += quantity;
            }            

            OnPropertyChanged("Inventory");
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);

            if (item == null)
            { }
            else
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

                OnPropertyChanged("Inventory");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
