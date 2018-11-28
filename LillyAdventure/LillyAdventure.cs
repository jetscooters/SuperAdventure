using System;
using Engine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LillyAdventure
{
    public partial class LillyAdventure : Form
    {
        private Player _player;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public LillyAdventure()
        {
            InitializeComponent();
            World.Initi();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = SaveLoad.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }
            
            //lblHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            lblHitPoints.Text = _player.CurrentHitPoints + " / " + _player.MaximumHitPoints;
            lblGold.DataBindings.Add("Text", _player, "Gold");
            lblExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", _player, "Level");

            //Handle updating inventory
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AutoGenerateColumns = false;

            dgvInventory.DataSource = _player.Inventory;

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "ItemName"
            });

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "#",
                DataPropertyName = "Quantity"
            });

            dgvInventory.ShowCellToolTips = true;
            dgvInventory.CellFormatting += OnInventoryRowAdd;

            //Handle updating quests
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.AutoGenerateColumns = false;

            dgvQuests.DataSource = _player.Quests;

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Name"
            });

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Done?",
                DataPropertyName = "IsCompleted"
            });

            dgvQuests.ShowCellToolTips = true;
            dgvQuests.CellFormatting += OnQuestRowAdd;

            cboWeapons.DataSource = _player.Weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "ID";

            if (_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            cboPotions.DataSource = _player.Potions;
            cboPotions.DisplayMember = "Name";
            cboPotions.ValueMember = "ID";

            cbTools.DataSource = _player.Tools;
            cbTools.DisplayMember = "Name";
            cbTools.ValueMember = "ID";

            _player.PropertyChanged += PlayerOnPropertyChanged;
            _player.OnMessage += Message;


            _player.MoveTo(_player.CurrentLocation);
        }


        private void OnInventoryRowAdd(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int row = e.RowIndex;
            int col = dgvInventory.Columns["Name"].Index;
            DataGridViewCell cell = dgvInventory[col, row];
            Item item = World.ItemByName(cell.Value.ToString());

            if(item != null)
            {
                cell.ToolTipText = item.Description;
            }
        }

        private void OnQuestRowAdd(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int row = e.RowIndex;
            int col = dgvQuests.Columns["Name"].Index;
            DataGridViewCell cell = dgvQuests[col, row];
            Quest item = World.QuestByName(cell.Value.ToString());

            if (item != null)
            {
                cell.ToolTipText = item.Description;
            }
        }

                private void btnNorth_Click(object sender, EventArgs e)
        {
            _player.MoveNorth();
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            _player.MoveEast();
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            _player.MoveSouth();
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            _player.MoveWest();
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            Weapon weapon = (Weapon)cboWeapons.SelectedItem;

            _player.UseWeapon(weapon);
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            _player.UsePotion(potion);
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                cboWeapons.DataSource = _player.Weapons;

                if (!_player.Inventory.Where(ii => ii.Details is Weapon && ii.Quantity > 0).Any())
                {
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
            }

            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.DataSource = _player.Potions;

                if (!_player.Inventory.Where(ii => ii.Details is HealingPotion && ii.Quantity > 0).Any())
                {
                    cboPotions.Visible = false;
                    btnUsePotion.Visible = false;
                }
            }

            if (propertyChangedEventArgs.PropertyName == "CurrentHitPoints" || propertyChangedEventArgs.PropertyName == "MaximumHitPoints")
            {
                lblHitPoints.Text = _player.CurrentHitPoints + " / " + _player.MaximumHitPoints;
            }

            if (propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                //Display relevant buttons
                btnNorth.Visible = _player.CurrentLocation.ToNorth != null;
                btnEast.Visible = _player.CurrentLocation.ToEast != null;
                btnSouth.Visible = _player.CurrentLocation.ToSouth != null;
                btnWest.Visible = _player.CurrentLocation.ToWest != null;

                //Display location information
                SetLocationText(_player.CurrentLocation);

                //Is there a vendor here?
                btnTrade.Visible = _player.CurrentLocation.VendorWorkingHere != null;                
            }

            if (propertyChangedEventArgs.PropertyName == "CurrentResource")
            {
                if (_player.CurrentResource == null)
                {
                    cbTools.Visible = false;
                    btnUseTool.Visible = false;
                }
                else
                {
                    cbTools.Visible = _player.Tools.Any();
                    btnUseTool.Visible = _player.Tools.Any();
                }
            }

            if (propertyChangedEventArgs.PropertyName == "CurrentMonster")
            {
                //update UI
                if (_player.CurrentMonster == null)
                {
                    cboWeapons.Visible = false;
                    cboPotions.Visible = false;
                    btnUseWeapon.Visible = false;
                    btnUsePotion.Visible = false;
                }
                else
                {
                    UpdateUI();
                }
            }
        }

        private void UpdateUI()
        {
            cboWeapons.Visible = _player.Weapons.Any();
            cboPotions.Visible = _player.Potions.Any();
            btnUseWeapon.Visible = _player.Weapons.Any();
            btnUsePotion.Visible = _player.Potions.Any();
        }

        private void SetLocationText(Location loc)
        {
            rtbLocation.Text = loc.Name + Environment.NewLine;
            rtbLocation.Text += loc.Description;
        }

        private void Message(object sender, MessageEventArgs message)
        {
            rtbMessages.Text += message.Message + Environment.NewLine;

            if (message.ExtraNewLine)
            {
                rtbMessages.Text += Environment.NewLine;
            }

            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void LillyAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }

        private void btnTrade_Click(object sender, EventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(_player);
            tradingScreen.StartPosition = FormStartPosition.CenterParent;
            tradingScreen.ShowDialog(this);
        }

        private void btnUseTool_Click(object sender, EventArgs e)
        {
            _player.UseTool((Tool)cbTools.SelectedItem);
        }

        private void cbTools_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
