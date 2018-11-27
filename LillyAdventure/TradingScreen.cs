using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace LillyAdventure
{
    public partial class TradingScreen : Form
    {
        private Player _player;
        private Vendor _vendor;

        public TradingScreen(Player player)
        {
            _player = player;
            _vendor = _player.CurrentLocation.VendorWorkingHere;

            InitializeComponent();

            //Style to display numeric column values
            DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
            rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            rightAlignedCellStyle.DataSourceNullValue = "0";
            rightAlignedCellStyle.NullValue = "0";

            //Populate datagrid for player
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;

            //This hidden column holds the item ID
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "ItemName"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ToSell",
                HeaderText = "To Sell",
                Width = 65,
                DefaultCellStyle = rightAlignedCellStyle,
                ReadOnly = false
            });

            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "SellButton",
                HeaderText = "",
                Text = "Sell",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });

            // Bind the player's inventory to the datagridview 
            dgvMyItems.DataSource = _player.Inventory;

            // When the user clicks on a row, call this function
            dgvMyItems.EditingControlShowing += OnTBTypeMine;
            dgvMyItems.CellClick += dgvMyItems_CellClick;


            // Populate the datagrid for the vendor's inventory
            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;

            // This hidden column holds the item ID, so we know which item to sell
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "ItemName"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 35,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ToBuy",
                HeaderText = "To Buy",
                Width = 65,
                DefaultCellStyle = rightAlignedCellStyle,                
                ReadOnly = false                
            });

            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {                
                Name = "BuyButton",
                HeaderText = "",
                Text = "Buy",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });

            // Bind the vendor's inventory to the datagridview 
            dgvVendorItems.DataSource = _vendor.Inventory;

            // When the user clicks on a row, call this function
            dgvVendorItems.EditingControlShowing += OnTBTypeVendor;
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;
        }

        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // The first column of a datagridview has a ColumnIndex = 0
            // This is known as a "zero-based" array/collection/list.
            // You start counting with 0.
            //
            // The 5th column (ColumnIndex = 4) is the column with the button.
            // So, if the player clicked the button column, we will sell an item from that row.

            int buttonCol = dgvMyItems.Columns["SellButton"].Index;
            int tbCol = dgvMyItems.Columns["ToSell"].Index;

            if (e.ColumnIndex == buttonCol)
            {
                // This gets the ID value of the item, from the hidden 1st column
                // Remember, ColumnIndex = 0, for the first column
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;
                var toSell = dgvMyItems.Rows[e.RowIndex].Cells[tbCol].Value;

                // Get the Item object for the selected item row
                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));
                int number = Convert.ToInt32(toSell);
                int playerHas = _player.Inventory.SingleOrDefault(ii => ii.ItemID == itemBeingSold.ID).Quantity;

                if (number > 0)
                {
                    if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                    {
                        MessageBox.Show("You cannot sell the " + itemBeingSold.Name);
                    }
                    else
                    {
                        if (playerHas >= number)
                        {
                            Sell(itemBeingSold, number);
                        }
                        else
                        {
                            Sell(itemBeingSold, playerHas);
                        }

                    }
                }                
            }
        }

        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int buttonCol = dgvVendorItems.Columns["BuyButton"].Index;
            int tbCol = dgvVendorItems.Columns["ToBuy"].Index;

            // The 4th column (ColumnIndex = 3) has the "Buy 1" button.
            if (e.ColumnIndex == buttonCol)
            {
                // This gets the ID value of the item, from the hidden 1st column
                var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;
                var toBuy = dgvVendorItems.Rows[e.RowIndex].Cells[tbCol].Value;

                // Get the Item object for the selected item row
                Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));
                int number = Convert.ToInt32(toBuy);                
                int vendorHas = _vendor.Inventory.SingleOrDefault(ii => ii.ItemID == itemBeingBought.ID).Quantity;

                if (number > 0)
                {
                    if (vendorHas >= number)
                    {
                        Buy(itemBeingBought, number);
                    }
                    else
                    {
                        Buy(itemBeingBought, vendorHas);
                    }
                }
            }
        }

        private void Buy(Item item, int number)
        {
            int total = item.Price * number;

            // Check if the player has enough gold to buy the item
            if (_player.Gold >= total)
            {
                // Add one of the items to the player's inventory
                _player.AddItemToInventory(item, number);

                // Remove the gold to pay for the item
                _player.Gold -= total;

                _vendor.RemoveItemFromInventory(item, number);
            }
            else
            {
                MessageBox.Show("You do not have enough gold to buy the " + item.Name + ".");
            }
        }

        private void Sell(Item item, int number)
        {
            int total = item.Price * number;

            _vendor.AddItemToInventory(item, number);
            _player.RemoveItemFromInventory(item, number);

            // Give the player the gold for the item being sold.
            _player.Gold += total;
        }

        private void OnTBTypeMine(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int col = dgvMyItems.Columns["ToSell"].Index;

            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dgvMyItems.CurrentCell.ColumnIndex == col) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void OnTBTypeVendor(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int col = dgvVendorItems.Columns["ToBuy"].Index;

            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dgvVendorItems.CurrentCell.ColumnIndex == col) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
