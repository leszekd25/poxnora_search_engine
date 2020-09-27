namespace poxnora_search_engine
{
    partial class CardRandomizer
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboMainFaction = new System.Windows.Forms.ComboBox();
            this.ComboSecondaryFaction = new System.Windows.Forms.ComboBox();
            this.PanelMainFactionCards = new System.Windows.Forms.Panel();
            this.MFCEquips = new System.Windows.Forms.TextBox();
            this.MFCRelics = new System.Windows.Forms.TextBox();
            this.MFCSpells = new System.Windows.Forms.TextBox();
            this.MFCChampions = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.PanelSecondaryFactionCards = new System.Windows.Forms.Panel();
            this.SFCEquips = new System.Windows.Forms.TextBox();
            this.SFCRelics = new System.Windows.Forms.TextBox();
            this.SFCSpells = new System.Windows.Forms.TextBox();
            this.SFCChampions = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.CheckAllowUnforgeableCards = new System.Windows.Forms.CheckBox();
            this.CheckHighlander = new System.Windows.Forms.CheckBox();
            this.ButtonGenerateCards = new System.Windows.Forms.Button();
            this.CheckboxAllowBannedCards = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.CheckAllowSplitCards = new System.Windows.Forms.CheckBox();
            this.PanelMainFactionCards.SuspendLayout();
            this.PanelSecondaryFactionCards.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Main faction";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(356, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Secondary faction";
            // 
            // ComboMainFaction
            // 
            this.ComboMainFaction.FormattingEnabled = true;
            this.ComboMainFaction.Items.AddRange(new object[] {
            "None",
            "Forglar Swamp",
            "Forsaken Wastes",
            "Ironfist Stronghold",
            "K\'thir Forest",
            "Sundered Lands",
            "Shattered Peaks",
            "Savage Tundra",
            "Underdepths"});
            this.ComboMainFaction.Location = new System.Drawing.Point(83, 6);
            this.ComboMainFaction.Name = "ComboMainFaction";
            this.ComboMainFaction.Size = new System.Drawing.Size(121, 21);
            this.ComboMainFaction.TabIndex = 2;
            this.ComboMainFaction.Text = "None";
            this.ComboMainFaction.SelectedIndexChanged += new System.EventHandler(this.ComboMainFaction_SelectedIndexChanged);
            // 
            // ComboSecondaryFaction
            // 
            this.ComboSecondaryFaction.FormattingEnabled = true;
            this.ComboSecondaryFaction.Items.AddRange(new object[] {
            "None",
            "Forglar Swamp",
            "Forsaken Wastes",
            "Ironfist Stronghold",
            "K\'thir Forest",
            "Sundered Lands",
            "Shattered Peaks",
            "Savage Tundra",
            "Underdepths"});
            this.ComboSecondaryFaction.Location = new System.Drawing.Point(455, 6);
            this.ComboSecondaryFaction.Name = "ComboSecondaryFaction";
            this.ComboSecondaryFaction.Size = new System.Drawing.Size(121, 21);
            this.ComboSecondaryFaction.TabIndex = 3;
            this.ComboSecondaryFaction.Text = "None";
            this.ComboSecondaryFaction.SelectedIndexChanged += new System.EventHandler(this.ComboSecondaryFaction_SelectedIndexChanged);
            // 
            // PanelMainFactionCards
            // 
            this.PanelMainFactionCards.Controls.Add(this.MFCEquips);
            this.PanelMainFactionCards.Controls.Add(this.MFCRelics);
            this.PanelMainFactionCards.Controls.Add(this.MFCSpells);
            this.PanelMainFactionCards.Controls.Add(this.MFCChampions);
            this.PanelMainFactionCards.Controls.Add(this.label3);
            this.PanelMainFactionCards.Enabled = false;
            this.PanelMainFactionCards.Location = new System.Drawing.Point(0, 60);
            this.PanelMainFactionCards.Name = "PanelMainFactionCards";
            this.PanelMainFactionCards.Size = new System.Drawing.Size(576, 37);
            this.PanelMainFactionCards.TabIndex = 4;
            // 
            // MFCEquips
            // 
            this.MFCEquips.Location = new System.Drawing.Point(475, 9);
            this.MFCEquips.Name = "MFCEquips";
            this.MFCEquips.Size = new System.Drawing.Size(61, 20);
            this.MFCEquips.TabIndex = 9;
            this.MFCEquips.Text = "0";
            // 
            // MFCRelics
            // 
            this.MFCRelics.Location = new System.Drawing.Point(359, 9);
            this.MFCRelics.Name = "MFCRelics";
            this.MFCRelics.Size = new System.Drawing.Size(61, 20);
            this.MFCRelics.TabIndex = 8;
            this.MFCRelics.Text = "0";
            // 
            // MFCSpells
            // 
            this.MFCSpells.Location = new System.Drawing.Point(249, 9);
            this.MFCSpells.Name = "MFCSpells";
            this.MFCSpells.Size = new System.Drawing.Size(61, 20);
            this.MFCSpells.TabIndex = 7;
            this.MFCSpells.Text = "0";
            // 
            // MFCChampions
            // 
            this.MFCChampions.Location = new System.Drawing.Point(143, 9);
            this.MFCChampions.Name = "MFCChampions";
            this.MFCChampions.Size = new System.Drawing.Size(61, 20);
            this.MFCChampions.TabIndex = 6;
            this.MFCChampions.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Main faction cards";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(140, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Champions";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(246, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Spells";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(356, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Relics";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(472, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Equips";
            // 
            // PanelSecondaryFactionCards
            // 
            this.PanelSecondaryFactionCards.Controls.Add(this.SFCEquips);
            this.PanelSecondaryFactionCards.Controls.Add(this.SFCRelics);
            this.PanelSecondaryFactionCards.Controls.Add(this.SFCSpells);
            this.PanelSecondaryFactionCards.Controls.Add(this.SFCChampions);
            this.PanelSecondaryFactionCards.Controls.Add(this.label8);
            this.PanelSecondaryFactionCards.Enabled = false;
            this.PanelSecondaryFactionCards.Location = new System.Drawing.Point(0, 103);
            this.PanelSecondaryFactionCards.Name = "PanelSecondaryFactionCards";
            this.PanelSecondaryFactionCards.Size = new System.Drawing.Size(576, 37);
            this.PanelSecondaryFactionCards.TabIndex = 10;
            // 
            // SFCEquips
            // 
            this.SFCEquips.Location = new System.Drawing.Point(475, 9);
            this.SFCEquips.Name = "SFCEquips";
            this.SFCEquips.Size = new System.Drawing.Size(61, 20);
            this.SFCEquips.TabIndex = 9;
            this.SFCEquips.Text = "0";
            // 
            // SFCRelics
            // 
            this.SFCRelics.Location = new System.Drawing.Point(359, 9);
            this.SFCRelics.Name = "SFCRelics";
            this.SFCRelics.Size = new System.Drawing.Size(61, 20);
            this.SFCRelics.TabIndex = 8;
            this.SFCRelics.Text = "0";
            // 
            // SFCSpells
            // 
            this.SFCSpells.Location = new System.Drawing.Point(249, 9);
            this.SFCSpells.Name = "SFCSpells";
            this.SFCSpells.Size = new System.Drawing.Size(61, 20);
            this.SFCSpells.TabIndex = 7;
            this.SFCSpells.Text = "0";
            // 
            // SFCChampions
            // 
            this.SFCChampions.Location = new System.Drawing.Point(143, 9);
            this.SFCChampions.Name = "SFCChampions";
            this.SFCChampions.Size = new System.Drawing.Size(61, 20);
            this.SFCChampions.TabIndex = 6;
            this.SFCChampions.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(122, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Secondary faction cards";
            // 
            // CheckAllowUnforgeableCards
            // 
            this.CheckAllowUnforgeableCards.AutoSize = true;
            this.CheckAllowUnforgeableCards.Location = new System.Drawing.Point(330, 150);
            this.CheckAllowUnforgeableCards.Name = "CheckAllowUnforgeableCards";
            this.CheckAllowUnforgeableCards.Size = new System.Drawing.Size(139, 17);
            this.CheckAllowUnforgeableCards.TabIndex = 14;
            this.CheckAllowUnforgeableCards.Text = "Allow unforgeable cards";
            this.CheckAllowUnforgeableCards.UseVisualStyleBackColor = true;
            // 
            // CheckHighlander
            // 
            this.CheckHighlander.AutoSize = true;
            this.CheckHighlander.Location = new System.Drawing.Point(15, 150);
            this.CheckHighlander.Name = "CheckHighlander";
            this.CheckHighlander.Size = new System.Drawing.Size(77, 17);
            this.CheckHighlander.TabIndex = 15;
            this.CheckHighlander.Text = "Highlander";
            this.CheckHighlander.UseVisualStyleBackColor = true;
            // 
            // ButtonGenerateCards
            // 
            this.ButtonGenerateCards.Location = new System.Drawing.Point(475, 146);
            this.ButtonGenerateCards.Name = "ButtonGenerateCards";
            this.ButtonGenerateCards.Size = new System.Drawing.Size(101, 23);
            this.ButtonGenerateCards.TabIndex = 18;
            this.ButtonGenerateCards.Text = "Generate cards";
            this.ButtonGenerateCards.UseVisualStyleBackColor = true;
            this.ButtonGenerateCards.Click += new System.EventHandler(this.ButtonGenerateCards_Click);
            // 
            // CheckboxAllowBannedCards
            // 
            this.CheckboxAllowBannedCards.AutoSize = true;
            this.CheckboxAllowBannedCards.Location = new System.Drawing.Point(98, 150);
            this.CheckboxAllowBannedCards.Name = "CheckboxAllowBannedCards";
            this.CheckboxAllowBannedCards.Size = new System.Drawing.Size(119, 17);
            this.CheckboxAllowBannedCards.TabIndex = 19;
            this.CheckboxAllowBannedCards.Text = "Allow banned cards";
            this.CheckboxAllowBannedCards.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 178);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(581, 22);
            this.statusStrip1.TabIndex = 20;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // CheckAllowSplitCards
            // 
            this.CheckAllowSplitCards.AutoSize = true;
            this.CheckAllowSplitCards.Location = new System.Drawing.Point(223, 150);
            this.CheckAllowSplitCards.Name = "CheckAllowSplitCards";
            this.CheckAllowSplitCards.Size = new System.Drawing.Size(101, 17);
            this.CheckAllowSplitCards.TabIndex = 21;
            this.CheckAllowSplitCards.Text = "Allow split cards";
            this.CheckAllowSplitCards.UseVisualStyleBackColor = true;
            // 
            // CardRandomizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 200);
            this.Controls.Add(this.CheckAllowSplitCards);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.CheckboxAllowBannedCards);
            this.Controls.Add(this.ButtonGenerateCards);
            this.Controls.Add(this.CheckHighlander);
            this.Controls.Add(this.CheckAllowUnforgeableCards);
            this.Controls.Add(this.PanelSecondaryFactionCards);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PanelMainFactionCards);
            this.Controls.Add(this.ComboSecondaryFaction);
            this.Controls.Add(this.ComboMainFaction);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CardRandomizer";
            this.ShowInTaskbar = false;
            this.Text = "CardRandomizer";
            this.Deactivate += new System.EventHandler(this.CardRandomizer_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CardRandomizer_FormClosing);
            this.Load += new System.EventHandler(this.CardRandomizer_Load);
            this.PanelMainFactionCards.ResumeLayout(false);
            this.PanelMainFactionCards.PerformLayout();
            this.PanelSecondaryFactionCards.ResumeLayout(false);
            this.PanelSecondaryFactionCards.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ComboMainFaction;
        private System.Windows.Forms.ComboBox ComboSecondaryFaction;
        private System.Windows.Forms.Panel PanelMainFactionCards;
        private System.Windows.Forms.TextBox MFCEquips;
        private System.Windows.Forms.TextBox MFCRelics;
        private System.Windows.Forms.TextBox MFCSpells;
        private System.Windows.Forms.TextBox MFCChampions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel PanelSecondaryFactionCards;
        private System.Windows.Forms.TextBox SFCEquips;
        private System.Windows.Forms.TextBox SFCRelics;
        private System.Windows.Forms.TextBox SFCSpells;
        private System.Windows.Forms.TextBox SFCChampions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox CheckAllowUnforgeableCards;
        private System.Windows.Forms.CheckBox CheckHighlander;
        private System.Windows.Forms.Button ButtonGenerateCards;
        private System.Windows.Forms.CheckBox CheckboxAllowBannedCards;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.CheckBox CheckAllowSplitCards;
    }
}