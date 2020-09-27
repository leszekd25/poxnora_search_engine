namespace poxnora_search_engine
{
    partial class ChampionBuilder
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TextDamage = new System.Windows.Forms.TextBox();
            this.TextSpeed = new System.Windows.Forms.TextBox();
            this.TextMinRange = new System.Windows.Forms.TextBox();
            this.TextMaxRange = new System.Windows.Forms.TextBox();
            this.TextDefense = new System.Windows.Forms.TextBox();
            this.TextHitPoints = new System.Windows.Forms.TextBox();
            this.TextSize = new System.Windows.Forms.TextBox();
            this.ListAbilities = new System.Windows.Forms.ListBox();
            this.TextAbility = new System.Windows.Forms.TextBox();
            this.AddAbility = new System.Windows.Forms.Button();
            this.RemoveAbility = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.ChampionTemplate = new System.Windows.Forms.TextBox();
            this.LoadFromChampions = new System.Windows.Forms.Button();
            this.ClearControls = new System.Windows.Forms.Button();
            this.TextStatsNoraCost = new System.Windows.Forms.Label();
            this.TextAbilityNoraCost = new System.Windows.Forms.Label();
            this.TextEstimatedNoraCost = new System.Windows.Forms.Label();
            this.TextLoadedChampionNoraCost = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Champion stats";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(351, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Champion abilities";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Damage";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Speed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Range";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Defense";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 192);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Hit points";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 218);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Size";
            // 
            // TextDamage
            // 
            this.TextDamage.Location = new System.Drawing.Point(102, 85);
            this.TextDamage.Name = "TextDamage";
            this.TextDamage.Size = new System.Drawing.Size(100, 20);
            this.TextDamage.TabIndex = 8;
            this.TextDamage.Validated += new System.EventHandler(this.TextDamage_Validated);
            // 
            // TextSpeed
            // 
            this.TextSpeed.Location = new System.Drawing.Point(102, 111);
            this.TextSpeed.Name = "TextSpeed";
            this.TextSpeed.Size = new System.Drawing.Size(100, 20);
            this.TextSpeed.TabIndex = 9;
            this.TextSpeed.Validated += new System.EventHandler(this.TextSpeed_Validated);
            // 
            // TextMinRange
            // 
            this.TextMinRange.Location = new System.Drawing.Point(102, 137);
            this.TextMinRange.Name = "TextMinRange";
            this.TextMinRange.Size = new System.Drawing.Size(100, 20);
            this.TextMinRange.TabIndex = 10;
            this.TextMinRange.Validated += new System.EventHandler(this.TextMinRange_Validated);
            // 
            // TextMaxRange
            // 
            this.TextMaxRange.Location = new System.Drawing.Point(228, 137);
            this.TextMaxRange.Name = "TextMaxRange";
            this.TextMaxRange.Size = new System.Drawing.Size(100, 20);
            this.TextMaxRange.TabIndex = 11;
            this.TextMaxRange.Validated += new System.EventHandler(this.TextMaxRange_Validated);
            // 
            // TextDefense
            // 
            this.TextDefense.Location = new System.Drawing.Point(102, 163);
            this.TextDefense.Name = "TextDefense";
            this.TextDefense.Size = new System.Drawing.Size(100, 20);
            this.TextDefense.TabIndex = 12;
            this.TextDefense.Validated += new System.EventHandler(this.TextDefense_Validated);
            // 
            // TextHitPoints
            // 
            this.TextHitPoints.Location = new System.Drawing.Point(102, 189);
            this.TextHitPoints.Name = "TextHitPoints";
            this.TextHitPoints.Size = new System.Drawing.Size(100, 20);
            this.TextHitPoints.TabIndex = 13;
            this.TextHitPoints.Validated += new System.EventHandler(this.TextHitPoints_Validated);
            // 
            // TextSize
            // 
            this.TextSize.Location = new System.Drawing.Point(102, 215);
            this.TextSize.Name = "TextSize";
            this.TextSize.Size = new System.Drawing.Size(100, 20);
            this.TextSize.TabIndex = 14;
            this.TextSize.Validated += new System.EventHandler(this.TextSize_Validated);
            // 
            // ListAbilities
            // 
            this.ListAbilities.FormattingEnabled = true;
            this.ListAbilities.Location = new System.Drawing.Point(354, 84);
            this.ListAbilities.Name = "ListAbilities";
            this.ListAbilities.Size = new System.Drawing.Size(219, 160);
            this.ListAbilities.TabIndex = 15;
            this.ListAbilities.SelectedIndexChanged += new System.EventHandler(this.ListAbilities_SelectedIndexChanged);
            // 
            // TextAbility
            // 
            this.TextAbility.Location = new System.Drawing.Point(354, 250);
            this.TextAbility.Name = "TextAbility";
            this.TextAbility.Size = new System.Drawing.Size(219, 20);
            this.TextAbility.TabIndex = 16;
            // 
            // AddAbility
            // 
            this.AddAbility.Location = new System.Drawing.Point(354, 276);
            this.AddAbility.Name = "AddAbility";
            this.AddAbility.Size = new System.Drawing.Size(75, 23);
            this.AddAbility.TabIndex = 17;
            this.AddAbility.Text = "Add";
            this.AddAbility.UseVisualStyleBackColor = true;
            this.AddAbility.Click += new System.EventHandler(this.AddAbility_Click);
            // 
            // RemoveAbility
            // 
            this.RemoveAbility.Enabled = false;
            this.RemoveAbility.Location = new System.Drawing.Point(498, 276);
            this.RemoveAbility.Name = "RemoveAbility";
            this.RemoveAbility.Size = new System.Drawing.Size(75, 23);
            this.RemoveAbility.TabIndex = 18;
            this.RemoveAbility.Text = "Remove";
            this.RemoveAbility.UseVisualStyleBackColor = true;
            this.RemoveAbility.Click += new System.EventHandler(this.RemoveAbility_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Load from champion";
            // 
            // ChampionTemplate
            // 
            this.ChampionTemplate.Location = new System.Drawing.Point(121, 6);
            this.ChampionTemplate.Name = "ChampionTemplate";
            this.ChampionTemplate.Size = new System.Drawing.Size(207, 20);
            this.ChampionTemplate.TabIndex = 20;
            // 
            // LoadFromChampions
            // 
            this.LoadFromChampions.Location = new System.Drawing.Point(354, 4);
            this.LoadFromChampions.Name = "LoadFromChampions";
            this.LoadFromChampions.Size = new System.Drawing.Size(75, 23);
            this.LoadFromChampions.TabIndex = 21;
            this.LoadFromChampions.Text = "Load";
            this.LoadFromChampions.UseVisualStyleBackColor = true;
            this.LoadFromChampions.Click += new System.EventHandler(this.LoadFromChampions_Click);
            // 
            // ClearControls
            // 
            this.ClearControls.Location = new System.Drawing.Point(498, 4);
            this.ClearControls.Name = "ClearControls";
            this.ClearControls.Size = new System.Drawing.Size(75, 23);
            this.ClearControls.TabIndex = 22;
            this.ClearControls.Text = "Clear";
            this.ClearControls.UseVisualStyleBackColor = true;
            this.ClearControls.Click += new System.EventHandler(this.ClearControls_Click);
            // 
            // TextStatsNoraCost
            // 
            this.TextStatsNoraCost.AutoSize = true;
            this.TextStatsNoraCost.Location = new System.Drawing.Point(225, 47);
            this.TextStatsNoraCost.Name = "TextStatsNoraCost";
            this.TextStatsNoraCost.Size = new System.Drawing.Size(41, 13);
            this.TextStatsNoraCost.TabIndex = 23;
            this.TextStatsNoraCost.Text = "label10";
            // 
            // TextAbilityNoraCost
            // 
            this.TextAbilityNoraCost.AutoSize = true;
            this.TextAbilityNoraCost.Location = new System.Drawing.Point(495, 47);
            this.TextAbilityNoraCost.Name = "TextAbilityNoraCost";
            this.TextAbilityNoraCost.Size = new System.Drawing.Size(41, 13);
            this.TextAbilityNoraCost.TabIndex = 24;
            this.TextAbilityNoraCost.Text = "label11";
            // 
            // TextEstimatedNoraCost
            // 
            this.TextEstimatedNoraCost.AutoSize = true;
            this.TextEstimatedNoraCost.Location = new System.Drawing.Point(351, 306);
            this.TextEstimatedNoraCost.Name = "TextEstimatedNoraCost";
            this.TextEstimatedNoraCost.Size = new System.Drawing.Size(41, 13);
            this.TextEstimatedNoraCost.TabIndex = 25;
            this.TextEstimatedNoraCost.Text = "label12";
            // 
            // TextLoadedChampionNoraCost
            // 
            this.TextLoadedChampionNoraCost.AutoSize = true;
            this.TextLoadedChampionNoraCost.Location = new System.Drawing.Point(12, 306);
            this.TextLoadedChampionNoraCost.Name = "TextLoadedChampionNoraCost";
            this.TextLoadedChampionNoraCost.Size = new System.Drawing.Size(41, 13);
            this.TextLoadedChampionNoraCost.TabIndex = 26;
            this.TextLoadedChampionNoraCost.Text = "label12";
            // 
            // ChampionBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 328);
            this.Controls.Add(this.TextLoadedChampionNoraCost);
            this.Controls.Add(this.TextEstimatedNoraCost);
            this.Controls.Add(this.TextAbilityNoraCost);
            this.Controls.Add(this.TextStatsNoraCost);
            this.Controls.Add(this.ClearControls);
            this.Controls.Add(this.LoadFromChampions);
            this.Controls.Add(this.ChampionTemplate);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.RemoveAbility);
            this.Controls.Add(this.AddAbility);
            this.Controls.Add(this.TextAbility);
            this.Controls.Add(this.ListAbilities);
            this.Controls.Add(this.TextSize);
            this.Controls.Add(this.TextHitPoints);
            this.Controls.Add(this.TextDefense);
            this.Controls.Add(this.TextMaxRange);
            this.Controls.Add(this.TextMinRange);
            this.Controls.Add(this.TextSpeed);
            this.Controls.Add(this.TextDamage);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChampionBuilder";
            this.Text = "ChampionBuilder";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ChampionBuilder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TextDamage;
        private System.Windows.Forms.TextBox TextSpeed;
        private System.Windows.Forms.TextBox TextMinRange;
        private System.Windows.Forms.TextBox TextMaxRange;
        private System.Windows.Forms.TextBox TextDefense;
        private System.Windows.Forms.TextBox TextHitPoints;
        private System.Windows.Forms.TextBox TextSize;
        private System.Windows.Forms.ListBox ListAbilities;
        private System.Windows.Forms.TextBox TextAbility;
        private System.Windows.Forms.Button AddAbility;
        private System.Windows.Forms.Button RemoveAbility;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox ChampionTemplate;
        private System.Windows.Forms.Button LoadFromChampions;
        private System.Windows.Forms.Button ClearControls;
        private System.Windows.Forms.Label TextStatsNoraCost;
        private System.Windows.Forms.Label TextAbilityNoraCost;
        private System.Windows.Forms.Label TextEstimatedNoraCost;
        private System.Windows.Forms.Label TextLoadedChampionNoraCost;
    }
}