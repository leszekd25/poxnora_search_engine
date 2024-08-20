
namespace poxnora_search_engine
{
    partial class PatchBuilderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListBoxEntries = new System.Windows.Forms.ListBox();
            this.ComboEntryMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TextBoxEntry = new System.Windows.Forms.TextBox();
            this.ButtonEntryAdd = new System.Windows.Forms.Button();
            this.ButtonEntryRemove = new System.Windows.Forms.Button();
            this.PanelEntryData = new System.Windows.Forms.Panel();
            this.PanelChampionAbilities = new System.Windows.Forms.Panel();
            this.LabelNoraRange = new System.Windows.Forms.Label();
            this.ComboU2 = new System.Windows.Forms.ComboBox();
            this.LabelDefaultCost = new System.Windows.Forms.Label();
            this.ComboU1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SetAbility = new System.Windows.Forms.Button();
            this.ButtonBaseAbilities = new System.Windows.Forms.Button();
            this.ButtonU1Abilities = new System.Windows.Forms.Button();
            this.ButtonU2Abilities = new System.Windows.Forms.Button();
            this.RemoveAbility = new System.Windows.Forms.Button();
            this.AddAbility = new System.Windows.Forms.Button();
            this.TextAbility = new System.Windows.Forms.TextBox();
            this.ListAbilities = new System.Windows.Forms.ListBox();
            this.PanelChampionPatchdata = new System.Windows.Forms.Panel();
            this.LabelBaseCost = new System.Windows.Forms.Label();
            this.LabelStatCost = new System.Windows.Forms.Label();
            this.TextNoraMod = new System.Windows.Forms.TextBox();
            this.TextHitPoints = new System.Windows.Forms.TextBox();
            this.TextDefense = new System.Windows.Forms.TextBox();
            this.TextMaxRange = new System.Windows.Forms.TextBox();
            this.TextMinRange = new System.Windows.Forms.TextBox();
            this.TextSpeed = new System.Windows.Forms.TextBox();
            this.TextDamage = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LabelChampionName = new System.Windows.Forms.Label();
            this.LabelChampionID = new System.Windows.Forms.Label();
            this.PanelChangeList = new System.Windows.Forms.Panel();
            this.ButtonPrevious = new System.Windows.Forms.Button();
            this.ButtonCurrent = new System.Windows.Forms.Button();
            this.ButtonGetJSON = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.RuneDescription = new poxnora_search_engine.Pox.RuneDescriptionControl();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.PanelEntryData.SuspendLayout();
            this.PanelChampionAbilities.SuspendLayout();
            this.PanelChampionPatchdata.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1202, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPatchToolStripMenuItem,
            this.loadPatchToolStripMenuItem,
            this.savePatchToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newPatchToolStripMenuItem
            // 
            this.newPatchToolStripMenuItem.Name = "newPatchToolStripMenuItem";
            this.newPatchToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.newPatchToolStripMenuItem.Text = "New patch";
            this.newPatchToolStripMenuItem.Click += new System.EventHandler(this.newPatchToolStripMenuItem_Click);
            // 
            // loadPatchToolStripMenuItem
            // 
            this.loadPatchToolStripMenuItem.Name = "loadPatchToolStripMenuItem";
            this.loadPatchToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.loadPatchToolStripMenuItem.Text = "Load patch";
            this.loadPatchToolStripMenuItem.Click += new System.EventHandler(this.loadPatchToolStripMenuItem_Click);
            // 
            // savePatchToolStripMenuItem
            // 
            this.savePatchToolStripMenuItem.Name = "savePatchToolStripMenuItem";
            this.savePatchToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.savePatchToolStripMenuItem.Text = "Save patch";
            this.savePatchToolStripMenuItem.Click += new System.EventHandler(this.savePatchToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ListBoxEntries
            // 
            this.ListBoxEntries.FormattingEnabled = true;
            this.ListBoxEntries.Location = new System.Drawing.Point(12, 54);
            this.ListBoxEntries.Name = "ListBoxEntries";
            this.ListBoxEntries.Size = new System.Drawing.Size(239, 329);
            this.ListBoxEntries.TabIndex = 1;
            this.ListBoxEntries.SelectedIndexChanged += new System.EventHandler(this.ListBoxEntries_SelectedIndexChanged);
            // 
            // ComboEntryMode
            // 
            this.ComboEntryMode.FormattingEnabled = true;
            this.ComboEntryMode.Items.AddRange(new object[] {
            "Champions"});
            this.ComboEntryMode.Location = new System.Drawing.Point(130, 27);
            this.ComboEntryMode.Name = "ComboEntryMode";
            this.ComboEntryMode.Size = new System.Drawing.Size(121, 21);
            this.ComboEntryMode.TabIndex = 2;
            this.ComboEntryMode.Text = "Champions";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Entry mode";
            // 
            // TextBoxEntry
            // 
            this.TextBoxEntry.Location = new System.Drawing.Point(12, 389);
            this.TextBoxEntry.Name = "TextBoxEntry";
            this.TextBoxEntry.Size = new System.Drawing.Size(239, 20);
            this.TextBoxEntry.TabIndex = 4;
            // 
            // ButtonEntryAdd
            // 
            this.ButtonEntryAdd.Location = new System.Drawing.Point(12, 415);
            this.ButtonEntryAdd.Name = "ButtonEntryAdd";
            this.ButtonEntryAdd.Size = new System.Drawing.Size(83, 23);
            this.ButtonEntryAdd.TabIndex = 5;
            this.ButtonEntryAdd.Text = "Add entry";
            this.ButtonEntryAdd.UseVisualStyleBackColor = true;
            this.ButtonEntryAdd.Click += new System.EventHandler(this.ButtonEntryAdd_Click);
            // 
            // ButtonEntryRemove
            // 
            this.ButtonEntryRemove.Location = new System.Drawing.Point(168, 415);
            this.ButtonEntryRemove.Name = "ButtonEntryRemove";
            this.ButtonEntryRemove.Size = new System.Drawing.Size(83, 23);
            this.ButtonEntryRemove.TabIndex = 6;
            this.ButtonEntryRemove.Text = "Remove entry";
            this.ButtonEntryRemove.UseVisualStyleBackColor = true;
            this.ButtonEntryRemove.Click += new System.EventHandler(this.ButtonEntryRemove_Click);
            // 
            // PanelEntryData
            // 
            this.PanelEntryData.Controls.Add(this.PanelChampionAbilities);
            this.PanelEntryData.Controls.Add(this.PanelChampionPatchdata);
            this.PanelEntryData.Enabled = false;
            this.PanelEntryData.Location = new System.Drawing.Point(257, 27);
            this.PanelEntryData.Name = "PanelEntryData";
            this.PanelEntryData.Size = new System.Drawing.Size(312, 647);
            this.PanelEntryData.TabIndex = 7;
            // 
            // PanelChampionAbilities
            // 
            this.PanelChampionAbilities.Controls.Add(this.LabelNoraRange);
            this.PanelChampionAbilities.Controls.Add(this.ComboU2);
            this.PanelChampionAbilities.Controls.Add(this.LabelDefaultCost);
            this.PanelChampionAbilities.Controls.Add(this.ComboU1);
            this.PanelChampionAbilities.Controls.Add(this.label2);
            this.PanelChampionAbilities.Controls.Add(this.label9);
            this.PanelChampionAbilities.Controls.Add(this.SetAbility);
            this.PanelChampionAbilities.Controls.Add(this.ButtonBaseAbilities);
            this.PanelChampionAbilities.Controls.Add(this.ButtonU1Abilities);
            this.PanelChampionAbilities.Controls.Add(this.ButtonU2Abilities);
            this.PanelChampionAbilities.Controls.Add(this.RemoveAbility);
            this.PanelChampionAbilities.Controls.Add(this.AddAbility);
            this.PanelChampionAbilities.Controls.Add(this.TextAbility);
            this.PanelChampionAbilities.Controls.Add(this.ListAbilities);
            this.PanelChampionAbilities.Location = new System.Drawing.Point(3, 262);
            this.PanelChampionAbilities.Name = "PanelChampionAbilities";
            this.PanelChampionAbilities.Size = new System.Drawing.Size(306, 382);
            this.PanelChampionAbilities.TabIndex = 1;
            // 
            // LabelNoraRange
            // 
            this.LabelNoraRange.AutoSize = true;
            this.LabelNoraRange.Location = new System.Drawing.Point(4, 347);
            this.LabelNoraRange.Name = "LabelNoraRange";
            this.LabelNoraRange.Size = new System.Drawing.Size(86, 13);
            this.LabelNoraRange.TabIndex = 31;
            this.LabelNoraRange.Text = "Nora cost range:";
            // 
            // ComboU2
            // 
            this.ComboU2.FormattingEnabled = true;
            this.ComboU2.Location = new System.Drawing.Point(107, 287);
            this.ComboU2.Name = "ComboU2";
            this.ComboU2.Size = new System.Drawing.Size(196, 21);
            this.ComboU2.TabIndex = 25;
            this.ComboU2.SelectionChangeCommitted += new System.EventHandler(this.ComboU2_SelectionChangeCommitted);
            // 
            // LabelDefaultCost
            // 
            this.LabelDefaultCost.AutoSize = true;
            this.LabelDefaultCost.Location = new System.Drawing.Point(3, 320);
            this.LabelDefaultCost.Name = "LabelDefaultCost";
            this.LabelDefaultCost.Size = new System.Drawing.Size(70, 13);
            this.LabelDefaultCost.TabIndex = 30;
            this.LabelDefaultCost.Text = "Default cost: ";
            // 
            // ComboU1
            // 
            this.ComboU1.FormattingEnabled = true;
            this.ComboU1.Location = new System.Drawing.Point(107, 260);
            this.ComboU1.Name = "ComboU1";
            this.ComboU1.Size = new System.Drawing.Size(196, 21);
            this.ComboU1.TabIndex = 24;
            this.ComboU1.SelectionChangeCommitted += new System.EventHandler(this.ComboU1_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 290);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "U2 default";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 263);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "U1 default";
            // 
            // SetAbility
            // 
            this.SetAbility.Location = new System.Drawing.Point(107, 228);
            this.SetAbility.Name = "SetAbility";
            this.SetAbility.Size = new System.Drawing.Size(95, 23);
            this.SetAbility.TabIndex = 23;
            this.SetAbility.Text = "Set";
            this.SetAbility.UseVisualStyleBackColor = true;
            this.SetAbility.Click += new System.EventHandler(this.SetAbility_Click);
            // 
            // ButtonBaseAbilities
            // 
            this.ButtonBaseAbilities.BackColor = System.Drawing.Color.Gold;
            this.ButtonBaseAbilities.Location = new System.Drawing.Point(3, 7);
            this.ButtonBaseAbilities.Name = "ButtonBaseAbilities";
            this.ButtonBaseAbilities.Size = new System.Drawing.Size(75, 23);
            this.ButtonBaseAbilities.TabIndex = 8;
            this.ButtonBaseAbilities.Text = "Base";
            this.ButtonBaseAbilities.UseVisualStyleBackColor = false;
            this.ButtonBaseAbilities.Click += new System.EventHandler(this.ButtonBaseAbilities_Click);
            // 
            // ButtonU1Abilities
            // 
            this.ButtonU1Abilities.Location = new System.Drawing.Point(147, 7);
            this.ButtonU1Abilities.Name = "ButtonU1Abilities";
            this.ButtonU1Abilities.Size = new System.Drawing.Size(75, 23);
            this.ButtonU1Abilities.TabIndex = 9;
            this.ButtonU1Abilities.Text = "U1";
            this.ButtonU1Abilities.UseVisualStyleBackColor = true;
            this.ButtonU1Abilities.Click += new System.EventHandler(this.ButtonU1Abilities_Click);
            // 
            // ButtonU2Abilities
            // 
            this.ButtonU2Abilities.Location = new System.Drawing.Point(228, 7);
            this.ButtonU2Abilities.Name = "ButtonU2Abilities";
            this.ButtonU2Abilities.Size = new System.Drawing.Size(75, 23);
            this.ButtonU2Abilities.TabIndex = 10;
            this.ButtonU2Abilities.Text = "U2";
            this.ButtonU2Abilities.UseVisualStyleBackColor = true;
            this.ButtonU2Abilities.Click += new System.EventHandler(this.ButtonU2Abilities_Click);
            // 
            // RemoveAbility
            // 
            this.RemoveAbility.Location = new System.Drawing.Point(208, 228);
            this.RemoveAbility.Name = "RemoveAbility";
            this.RemoveAbility.Size = new System.Drawing.Size(95, 23);
            this.RemoveAbility.TabIndex = 22;
            this.RemoveAbility.Text = "Remove";
            this.RemoveAbility.UseVisualStyleBackColor = true;
            this.RemoveAbility.Click += new System.EventHandler(this.RemoveAbility_Click);
            // 
            // AddAbility
            // 
            this.AddAbility.Location = new System.Drawing.Point(3, 228);
            this.AddAbility.Name = "AddAbility";
            this.AddAbility.Size = new System.Drawing.Size(98, 23);
            this.AddAbility.TabIndex = 21;
            this.AddAbility.Text = "Add";
            this.AddAbility.UseVisualStyleBackColor = true;
            this.AddAbility.Click += new System.EventHandler(this.AddAbility_Click);
            // 
            // TextAbility
            // 
            this.TextAbility.Location = new System.Drawing.Point(3, 202);
            this.TextAbility.Name = "TextAbility";
            this.TextAbility.Size = new System.Drawing.Size(300, 20);
            this.TextAbility.TabIndex = 20;
            // 
            // ListAbilities
            // 
            this.ListAbilities.FormattingEnabled = true;
            this.ListAbilities.Location = new System.Drawing.Point(3, 36);
            this.ListAbilities.Name = "ListAbilities";
            this.ListAbilities.Size = new System.Drawing.Size(300, 160);
            this.ListAbilities.TabIndex = 19;
            // 
            // PanelChampionPatchdata
            // 
            this.PanelChampionPatchdata.Controls.Add(this.LabelBaseCost);
            this.PanelChampionPatchdata.Controls.Add(this.LabelStatCost);
            this.PanelChampionPatchdata.Controls.Add(this.TextNoraMod);
            this.PanelChampionPatchdata.Controls.Add(this.TextHitPoints);
            this.PanelChampionPatchdata.Controls.Add(this.TextDefense);
            this.PanelChampionPatchdata.Controls.Add(this.TextMaxRange);
            this.PanelChampionPatchdata.Controls.Add(this.TextMinRange);
            this.PanelChampionPatchdata.Controls.Add(this.TextSpeed);
            this.PanelChampionPatchdata.Controls.Add(this.TextDamage);
            this.PanelChampionPatchdata.Controls.Add(this.label8);
            this.PanelChampionPatchdata.Controls.Add(this.label7);
            this.PanelChampionPatchdata.Controls.Add(this.label6);
            this.PanelChampionPatchdata.Controls.Add(this.label5);
            this.PanelChampionPatchdata.Controls.Add(this.label4);
            this.PanelChampionPatchdata.Controls.Add(this.label3);
            this.PanelChampionPatchdata.Controls.Add(this.LabelChampionName);
            this.PanelChampionPatchdata.Controls.Add(this.LabelChampionID);
            this.PanelChampionPatchdata.Location = new System.Drawing.Point(3, 3);
            this.PanelChampionPatchdata.Name = "PanelChampionPatchdata";
            this.PanelChampionPatchdata.Size = new System.Drawing.Size(306, 253);
            this.PanelChampionPatchdata.TabIndex = 0;
            // 
            // LabelBaseCost
            // 
            this.LabelBaseCost.AutoSize = true;
            this.LabelBaseCost.Location = new System.Drawing.Point(145, 201);
            this.LabelBaseCost.Name = "LabelBaseCost";
            this.LabelBaseCost.Size = new System.Drawing.Size(57, 13);
            this.LabelBaseCost.TabIndex = 29;
            this.LabelBaseCost.Text = "Base cost:";
            // 
            // LabelStatCost
            // 
            this.LabelStatCost.AutoSize = true;
            this.LabelStatCost.Location = new System.Drawing.Point(3, 201);
            this.LabelStatCost.Name = "LabelStatCost";
            this.LabelStatCost.Size = new System.Drawing.Size(52, 13);
            this.LabelStatCost.TabIndex = 28;
            this.LabelStatCost.Text = "Stat cost:";
            // 
            // TextNoraMod
            // 
            this.TextNoraMod.Location = new System.Drawing.Point(70, 167);
            this.TextNoraMod.Name = "TextNoraMod";
            this.TextNoraMod.Size = new System.Drawing.Size(100, 20);
            this.TextNoraMod.TabIndex = 27;
            this.TextNoraMod.Validated += new System.EventHandler(this.TextNoraMod_Validated);
            // 
            // TextHitPoints
            // 
            this.TextHitPoints.Location = new System.Drawing.Point(70, 141);
            this.TextHitPoints.Name = "TextHitPoints";
            this.TextHitPoints.Size = new System.Drawing.Size(100, 20);
            this.TextHitPoints.TabIndex = 26;
            this.TextHitPoints.Validated += new System.EventHandler(this.TextHitPoints_Validated);
            // 
            // TextDefense
            // 
            this.TextDefense.Location = new System.Drawing.Point(70, 115);
            this.TextDefense.Name = "TextDefense";
            this.TextDefense.Size = new System.Drawing.Size(100, 20);
            this.TextDefense.TabIndex = 25;
            this.TextDefense.Validated += new System.EventHandler(this.TextDefense_Validated);
            // 
            // TextMaxRange
            // 
            this.TextMaxRange.Location = new System.Drawing.Point(196, 89);
            this.TextMaxRange.Name = "TextMaxRange";
            this.TextMaxRange.Size = new System.Drawing.Size(100, 20);
            this.TextMaxRange.TabIndex = 24;
            this.TextMaxRange.Validated += new System.EventHandler(this.TextMaxRange_Validated);
            // 
            // TextMinRange
            // 
            this.TextMinRange.Location = new System.Drawing.Point(70, 89);
            this.TextMinRange.Name = "TextMinRange";
            this.TextMinRange.Size = new System.Drawing.Size(100, 20);
            this.TextMinRange.TabIndex = 23;
            this.TextMinRange.Validated += new System.EventHandler(this.TextMinRange_Validated);
            // 
            // TextSpeed
            // 
            this.TextSpeed.Location = new System.Drawing.Point(70, 63);
            this.TextSpeed.Name = "TextSpeed";
            this.TextSpeed.Size = new System.Drawing.Size(100, 20);
            this.TextSpeed.TabIndex = 22;
            this.TextSpeed.Validated += new System.EventHandler(this.TextSpeed_Validated);
            // 
            // TextDamage
            // 
            this.TextDamage.Location = new System.Drawing.Point(70, 37);
            this.TextDamage.Name = "TextDamage";
            this.TextDamage.Size = new System.Drawing.Size(100, 20);
            this.TextDamage.TabIndex = 21;
            this.TextDamage.Validated += new System.EventHandler(this.TextDamage_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Nora mod";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Hit points";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Defense";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Range";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Speed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Damage";
            // 
            // LabelChampionName
            // 
            this.LabelChampionName.AutoSize = true;
            this.LabelChampionName.Location = new System.Drawing.Point(81, 5);
            this.LabelChampionName.Name = "LabelChampionName";
            this.LabelChampionName.Size = new System.Drawing.Size(41, 13);
            this.LabelChampionName.TabIndex = 1;
            this.LabelChampionName.Text = "Name: ";
            // 
            // LabelChampionID
            // 
            this.LabelChampionID.AutoSize = true;
            this.LabelChampionID.Location = new System.Drawing.Point(3, 5);
            this.LabelChampionID.Name = "LabelChampionID";
            this.LabelChampionID.Size = new System.Drawing.Size(24, 13);
            this.LabelChampionID.TabIndex = 0;
            this.LabelChampionID.Text = "ID: ";
            // 
            // PanelChangeList
            // 
            this.PanelChangeList.Location = new System.Drawing.Point(575, 27);
            this.PanelChangeList.Name = "PanelChangeList";
            this.PanelChangeList.Size = new System.Drawing.Size(336, 458);
            this.PanelChangeList.TabIndex = 8;
            // 
            // ButtonPrevious
            // 
            this.ButtonPrevious.Location = new System.Drawing.Point(575, 491);
            this.ButtonPrevious.Name = "ButtonPrevious";
            this.ButtonPrevious.Size = new System.Drawing.Size(120, 23);
            this.ButtonPrevious.TabIndex = 0;
            this.ButtonPrevious.Text = "Previous";
            this.ButtonPrevious.UseVisualStyleBackColor = true;
            this.ButtonPrevious.Click += new System.EventHandler(this.ButtonPrevious_Click);
            // 
            // ButtonCurrent
            // 
            this.ButtonCurrent.Location = new System.Drawing.Point(780, 491);
            this.ButtonCurrent.Name = "ButtonCurrent";
            this.ButtonCurrent.Size = new System.Drawing.Size(131, 23);
            this.ButtonCurrent.TabIndex = 1;
            this.ButtonCurrent.Text = "Current";
            this.ButtonCurrent.UseVisualStyleBackColor = true;
            this.ButtonCurrent.Click += new System.EventHandler(this.ButtonCurrent_Click);
            // 
            // ButtonGetJSON
            // 
            this.ButtonGetJSON.Location = new System.Drawing.Point(12, 444);
            this.ButtonGetJSON.Name = "ButtonGetJSON";
            this.ButtonGetJSON.Size = new System.Drawing.Size(239, 23);
            this.ButtonGetJSON.TabIndex = 10;
            this.ButtonGetJSON.Text = "Convert to JSON";
            this.ButtonGetJSON.UseVisualStyleBackColor = true;
            this.ButtonGetJSON.Click += new System.EventHandler(this.ButtonGetJSON_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 681);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1202, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // RuneDescription
            // 
            this.RuneDescription.AutoScroll = true;
            this.RuneDescription.BackColor = System.Drawing.Color.Black;
            this.RuneDescription.Location = new System.Drawing.Point(917, 27);
            this.RuneDescription.Name = "RuneDescription";
            this.RuneDescription.Size = new System.Drawing.Size(281, 644);
            this.RuneDescription.TabIndex = 9;
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(39, 17);
            this.StatusText.Text = "Ready";
            // 
            // PatchBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 703);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ButtonGetJSON);
            this.Controls.Add(this.RuneDescription);
            this.Controls.Add(this.ButtonPrevious);
            this.Controls.Add(this.ButtonCurrent);
            this.Controls.Add(this.PanelChangeList);
            this.Controls.Add(this.PanelEntryData);
            this.Controls.Add(this.ButtonEntryRemove);
            this.Controls.Add(this.ButtonEntryAdd);
            this.Controls.Add(this.TextBoxEntry);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboEntryMode);
            this.Controls.Add(this.ListBoxEntries);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "PatchBuilderForm";
            this.Text = "Patch Builder - *untitled*";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PatchBuilderForm_FormClosed);
            this.Load += new System.EventHandler(this.PatchBuilderForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.PanelEntryData.ResumeLayout(false);
            this.PanelChampionAbilities.ResumeLayout(false);
            this.PanelChampionAbilities.PerformLayout();
            this.PanelChampionPatchdata.ResumeLayout(false);
            this.PanelChampionPatchdata.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newPatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ListBox ListBoxEntries;
        private System.Windows.Forms.ComboBox ComboEntryMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBoxEntry;
        private System.Windows.Forms.Button ButtonEntryAdd;
        private System.Windows.Forms.Button ButtonEntryRemove;
        private System.Windows.Forms.Panel PanelEntryData;
        private System.Windows.Forms.Panel PanelChampionPatchdata;
        private System.Windows.Forms.Label LabelChampionName;
        private System.Windows.Forms.Label LabelChampionID;
        private System.Windows.Forms.TextBox TextNoraMod;
        private System.Windows.Forms.TextBox TextHitPoints;
        private System.Windows.Forms.TextBox TextDefense;
        private System.Windows.Forms.TextBox TextMaxRange;
        private System.Windows.Forms.TextBox TextMinRange;
        private System.Windows.Forms.TextBox TextSpeed;
        private System.Windows.Forms.TextBox TextDamage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel PanelChampionAbilities;
        private System.Windows.Forms.ComboBox ComboU2;
        private System.Windows.Forms.ComboBox ComboU1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button SetAbility;
        private System.Windows.Forms.Button ButtonBaseAbilities;
        private System.Windows.Forms.Button ButtonU1Abilities;
        private System.Windows.Forms.Button ButtonU2Abilities;
        private System.Windows.Forms.Button RemoveAbility;
        private System.Windows.Forms.Button AddAbility;
        private System.Windows.Forms.TextBox TextAbility;
        private System.Windows.Forms.ListBox ListAbilities;
        private System.Windows.Forms.Label LabelStatCost;
        private System.Windows.Forms.Panel PanelChangeList;
        private System.Windows.Forms.Button ButtonPrevious;
        private System.Windows.Forms.Button ButtonCurrent;
        private Pox.RuneDescriptionControl RuneDescription;
        private System.Windows.Forms.Button ButtonGetJSON;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label LabelBaseCost;
        private System.Windows.Forms.Label LabelNoraRange;
        private System.Windows.Forms.Label LabelDefaultCost;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
    }
}