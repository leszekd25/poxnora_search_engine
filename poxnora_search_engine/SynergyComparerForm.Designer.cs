namespace poxnora_search_engine
{
    partial class SynergyComparerForm
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
            this.ComboFaction1 = new System.Windows.Forms.ComboBox();
            this.LabelFaction1 = new System.Windows.Forms.Label();
            this.LabelFaction2 = new System.Windows.Forms.Label();
            this.ComboFaction2 = new System.Windows.Forms.ComboBox();
            this.GridSynergies = new System.Windows.Forms.DataGridView();
            this.KeyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OldAvgPower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewAvgPower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OldMaxPower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewMaxPower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RadioGlobal = new System.Windows.Forms.RadioButton();
            this.RadioClassFactions = new System.Windows.Forms.RadioButton();
            this.RadioRaceFactions = new System.Windows.Forms.RadioButton();
            this.RadioFactionRaces = new System.Windows.Forms.RadioButton();
            this.RadioFactionClasses = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.LabelRaceClass = new System.Windows.Forms.Label();
            this.ComboRaceClass = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.GridSynergies)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ComboFaction1
            // 
            this.ComboFaction1.FormattingEnabled = true;
            this.ComboFaction1.Location = new System.Drawing.Point(96, 50);
            this.ComboFaction1.Name = "ComboFaction1";
            this.ComboFaction1.Size = new System.Drawing.Size(121, 21);
            this.ComboFaction1.TabIndex = 0;
            this.ComboFaction1.Visible = false;
            this.ComboFaction1.SelectedIndexChanged += new System.EventHandler(this.ComboFaction1_SelectedIndexChanged);
            // 
            // LabelFaction1
            // 
            this.LabelFaction1.AutoSize = true;
            this.LabelFaction1.Location = new System.Drawing.Point(12, 53);
            this.LabelFaction1.Name = "LabelFaction1";
            this.LabelFaction1.Size = new System.Drawing.Size(51, 13);
            this.LabelFaction1.TabIndex = 1;
            this.LabelFaction1.Text = "Faction 1";
            this.LabelFaction1.Visible = false;
            // 
            // LabelFaction2
            // 
            this.LabelFaction2.AutoSize = true;
            this.LabelFaction2.Location = new System.Drawing.Point(611, 53);
            this.LabelFaction2.Name = "LabelFaction2";
            this.LabelFaction2.Size = new System.Drawing.Size(51, 13);
            this.LabelFaction2.TabIndex = 3;
            this.LabelFaction2.Text = "Faction 2";
            this.LabelFaction2.Visible = false;
            // 
            // ComboFaction2
            // 
            this.ComboFaction2.FormattingEnabled = true;
            this.ComboFaction2.Location = new System.Drawing.Point(455, 50);
            this.ComboFaction2.Name = "ComboFaction2";
            this.ComboFaction2.Size = new System.Drawing.Size(121, 21);
            this.ComboFaction2.TabIndex = 2;
            this.ComboFaction2.Visible = false;
            this.ComboFaction2.SelectedIndexChanged += new System.EventHandler(this.ComboFaction2_SelectedIndexChanged);
            // 
            // GridSynergies
            // 
            this.GridSynergies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridSynergies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeyColumn,
            this.OldAvgPower,
            this.NewAvgPower,
            this.OldMaxPower,
            this.NewMaxPower});
            this.GridSynergies.Location = new System.Drawing.Point(12, 77);
            this.GridSynergies.Name = "GridSynergies";
            this.GridSynergies.Size = new System.Drawing.Size(650, 555);
            this.GridSynergies.TabIndex = 4;
            // 
            // KeyColumn
            // 
            this.KeyColumn.HeaderText = "(unused)";
            this.KeyColumn.Name = "KeyColumn";
            this.KeyColumn.Width = 200;
            // 
            // OldAvgPower
            // 
            this.OldAvgPower.HeaderText = "Old average power";
            this.OldAvgPower.Name = "OldAvgPower";
            // 
            // NewAvgPower
            // 
            this.NewAvgPower.HeaderText = "New average power";
            this.NewAvgPower.Name = "NewAvgPower";
            // 
            // OldMaxPower
            // 
            this.OldMaxPower.HeaderText = "Old max power";
            this.OldMaxPower.Name = "OldMaxPower";
            // 
            // NewMaxPower
            // 
            this.NewMaxPower.HeaderText = "New max power";
            this.NewMaxPower.Name = "NewMaxPower";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RadioGlobal);
            this.panel1.Controls.Add(this.RadioClassFactions);
            this.panel1.Controls.Add(this.RadioRaceFactions);
            this.panel1.Controls.Add(this.RadioFactionRaces);
            this.panel1.Controls.Add(this.RadioFactionClasses);
            this.panel1.Location = new System.Drawing.Point(96, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(461, 33);
            this.panel1.TabIndex = 5;
            // 
            // RadioGlobal
            // 
            this.RadioGlobal.AutoSize = true;
            this.RadioGlobal.Checked = true;
            this.RadioGlobal.Location = new System.Drawing.Point(395, 8);
            this.RadioGlobal.Name = "RadioGlobal";
            this.RadioGlobal.Size = new System.Drawing.Size(55, 17);
            this.RadioGlobal.TabIndex = 10;
            this.RadioGlobal.TabStop = true;
            this.RadioGlobal.Text = "Global";
            this.RadioGlobal.UseVisualStyleBackColor = true;
            this.RadioGlobal.CheckedChanged += new System.EventHandler(this.RadioGlobal_CheckedChanged);
            // 
            // RadioClassFactions
            // 
            this.RadioClassFactions.AutoSize = true;
            this.RadioClassFactions.Location = new System.Drawing.Point(299, 8);
            this.RadioClassFactions.Name = "RadioClassFactions";
            this.RadioClassFactions.Size = new System.Drawing.Size(90, 17);
            this.RadioClassFactions.TabIndex = 9;
            this.RadioClassFactions.Text = "Class factions";
            this.RadioClassFactions.UseVisualStyleBackColor = true;
            this.RadioClassFactions.CheckedChanged += new System.EventHandler(this.RadioGlobal_CheckedChanged);
            // 
            // RadioRaceFactions
            // 
            this.RadioRaceFactions.AutoSize = true;
            this.RadioRaceFactions.Location = new System.Drawing.Point(202, 8);
            this.RadioRaceFactions.Name = "RadioRaceFactions";
            this.RadioRaceFactions.Size = new System.Drawing.Size(91, 17);
            this.RadioRaceFactions.TabIndex = 8;
            this.RadioRaceFactions.Text = "Race factions";
            this.RadioRaceFactions.UseVisualStyleBackColor = true;
            this.RadioRaceFactions.CheckedChanged += new System.EventHandler(this.RadioGlobal_CheckedChanged);
            // 
            // RadioFactionRaces
            // 
            this.RadioFactionRaces.AutoSize = true;
            this.RadioFactionRaces.Location = new System.Drawing.Point(107, 8);
            this.RadioFactionRaces.Name = "RadioFactionRaces";
            this.RadioFactionRaces.Size = new System.Drawing.Size(89, 17);
            this.RadioFactionRaces.TabIndex = 7;
            this.RadioFactionRaces.Text = "Faction races";
            this.RadioFactionRaces.UseVisualStyleBackColor = true;
            this.RadioFactionRaces.CheckedChanged += new System.EventHandler(this.RadioGlobal_CheckedChanged);
            // 
            // RadioFactionClasses
            // 
            this.RadioFactionClasses.AutoSize = true;
            this.RadioFactionClasses.Location = new System.Drawing.Point(3, 8);
            this.RadioFactionClasses.Name = "RadioFactionClasses";
            this.RadioFactionClasses.Size = new System.Drawing.Size(98, 17);
            this.RadioFactionClasses.TabIndex = 6;
            this.RadioFactionClasses.Text = "Faction classes";
            this.RadioFactionClasses.UseVisualStyleBackColor = true;
            this.RadioFactionClasses.CheckedChanged += new System.EventHandler(this.RadioGlobal_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Mode";
            // 
            // LabelRaceClass
            // 
            this.LabelRaceClass.AutoSize = true;
            this.LabelRaceClass.Location = new System.Drawing.Point(214, 53);
            this.LabelRaceClass.Name = "LabelRaceClass";
            this.LabelRaceClass.Size = new System.Drawing.Size(33, 13);
            this.LabelRaceClass.TabIndex = 7;
            this.LabelRaceClass.Text = "Race";
            this.LabelRaceClass.Visible = false;
            // 
            // ComboRaceClass
            // 
            this.ComboRaceClass.FormattingEnabled = true;
            this.ComboRaceClass.Location = new System.Drawing.Point(263, 50);
            this.ComboRaceClass.Name = "ComboRaceClass";
            this.ComboRaceClass.Size = new System.Drawing.Size(121, 21);
            this.ComboRaceClass.TabIndex = 6;
            this.ComboRaceClass.Visible = false;
            this.ComboRaceClass.SelectedIndexChanged += new System.EventHandler(this.ComboRaceClass_SelectedIndexChanged);
            // 
            // SynergyComparerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 644);
            this.Controls.Add(this.LabelRaceClass);
            this.Controls.Add(this.ComboRaceClass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.GridSynergies);
            this.Controls.Add(this.LabelFaction2);
            this.Controls.Add(this.ComboFaction2);
            this.Controls.Add(this.LabelFaction1);
            this.Controls.Add(this.ComboFaction1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SynergyComparerForm";
            this.Text = "SynergyComparerForm";
            ((System.ComponentModel.ISupportInitialize)(this.GridSynergies)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ComboFaction1;
        private System.Windows.Forms.Label LabelFaction1;
        private System.Windows.Forms.Label LabelFaction2;
        private System.Windows.Forms.ComboBox ComboFaction2;
        private System.Windows.Forms.DataGridView GridSynergies;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton RadioFactionClasses;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn OldAvgPower;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewAvgPower;
        private System.Windows.Forms.DataGridViewTextBoxColumn OldMaxPower;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewMaxPower;
        private System.Windows.Forms.RadioButton RadioGlobal;
        private System.Windows.Forms.RadioButton RadioClassFactions;
        private System.Windows.Forms.RadioButton RadioRaceFactions;
        private System.Windows.Forms.RadioButton RadioFactionRaces;
        private System.Windows.Forms.Label LabelRaceClass;
        private System.Windows.Forms.ComboBox ComboRaceClass;
    }
}