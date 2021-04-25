namespace poxnora_search_engine.Pox
{
    partial class QuickFilterForm
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
            this.ListFactions = new System.Windows.Forms.CheckedListBox();
            this.ListClasses = new System.Windows.Forms.CheckedListBox();
            this.ListRaces = new System.Windows.Forms.CheckedListBox();
            this.ListRarities = new System.Windows.Forms.CheckedListBox();
            this.ListExpansions = new System.Windows.Forms.CheckedListBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ClearFactions = new System.Windows.Forms.Button();
            this.ToggleFactions = new System.Windows.Forms.Button();
            this.ClearClasses = new System.Windows.Forms.Button();
            this.ToggleClasses = new System.Windows.Forms.Button();
            this.ClearRaces = new System.Windows.Forms.Button();
            this.ToggleRaces = new System.Windows.Forms.Button();
            this.ClearRarities = new System.Windows.Forms.Button();
            this.ToggleRarities = new System.Windows.Forms.Button();
            this.ClearExpansions = new System.Windows.Forms.Button();
            this.ToggleExpansions = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListFactions
            // 
            this.ListFactions.CheckOnClick = true;
            this.ListFactions.FormattingEnabled = true;
            this.ListFactions.Location = new System.Drawing.Point(14, 27);
            this.ListFactions.Name = "ListFactions";
            this.ListFactions.Size = new System.Drawing.Size(197, 334);
            this.ListFactions.TabIndex = 0;
            // 
            // ListClasses
            // 
            this.ListClasses.CheckOnClick = true;
            this.ListClasses.FormattingEnabled = true;
            this.ListClasses.Location = new System.Drawing.Point(217, 27);
            this.ListClasses.Name = "ListClasses";
            this.ListClasses.Size = new System.Drawing.Size(197, 334);
            this.ListClasses.TabIndex = 1;
            // 
            // ListRaces
            // 
            this.ListRaces.CheckOnClick = true;
            this.ListRaces.FormattingEnabled = true;
            this.ListRaces.Location = new System.Drawing.Point(420, 27);
            this.ListRaces.Name = "ListRaces";
            this.ListRaces.Size = new System.Drawing.Size(197, 334);
            this.ListRaces.TabIndex = 2;
            // 
            // ListRarities
            // 
            this.ListRarities.CheckOnClick = true;
            this.ListRarities.FormattingEnabled = true;
            this.ListRarities.Location = new System.Drawing.Point(623, 27);
            this.ListRarities.Name = "ListRarities";
            this.ListRarities.Size = new System.Drawing.Size(197, 334);
            this.ListRarities.TabIndex = 3;
            // 
            // ListExpansions
            // 
            this.ListExpansions.CheckOnClick = true;
            this.ListExpansions.FormattingEnabled = true;
            this.ListExpansions.Location = new System.Drawing.Point(826, 27);
            this.ListExpansions.Name = "ListExpansions";
            this.ListExpansions.Size = new System.Drawing.Size(197, 334);
            this.ListExpansions.TabIndex = 4;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(15, 402);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 5;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(949, 402);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 6;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Faction";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(214, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Class";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(417, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Race";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(620, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Rarity";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(823, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Expansion";
            // 
            // ClearFactions
            // 
            this.ClearFactions.Location = new System.Drawing.Point(14, 367);
            this.ClearFactions.Name = "ClearFactions";
            this.ClearFactions.Size = new System.Drawing.Size(75, 23);
            this.ClearFactions.TabIndex = 12;
            this.ClearFactions.Text = "Clear";
            this.ClearFactions.UseVisualStyleBackColor = true;
            this.ClearFactions.Click += new System.EventHandler(this.ClearFactions_Click);
            // 
            // ToggleFactions
            // 
            this.ToggleFactions.Location = new System.Drawing.Point(136, 367);
            this.ToggleFactions.Name = "ToggleFactions";
            this.ToggleFactions.Size = new System.Drawing.Size(75, 23);
            this.ToggleFactions.TabIndex = 13;
            this.ToggleFactions.Text = "Toggle";
            this.ToggleFactions.UseVisualStyleBackColor = true;
            this.ToggleFactions.Click += new System.EventHandler(this.ToggleFactions_Click);
            // 
            // ClearClasses
            // 
            this.ClearClasses.Location = new System.Drawing.Point(217, 367);
            this.ClearClasses.Name = "ClearClasses";
            this.ClearClasses.Size = new System.Drawing.Size(75, 23);
            this.ClearClasses.TabIndex = 14;
            this.ClearClasses.Text = "Clear";
            this.ClearClasses.UseVisualStyleBackColor = true;
            this.ClearClasses.Click += new System.EventHandler(this.ClearClasses_Click);
            // 
            // ToggleClasses
            // 
            this.ToggleClasses.Location = new System.Drawing.Point(339, 367);
            this.ToggleClasses.Name = "ToggleClasses";
            this.ToggleClasses.Size = new System.Drawing.Size(75, 23);
            this.ToggleClasses.TabIndex = 15;
            this.ToggleClasses.Text = "Toggle";
            this.ToggleClasses.UseVisualStyleBackColor = true;
            this.ToggleClasses.Click += new System.EventHandler(this.ToggleClasses_Click);
            // 
            // ClearRaces
            // 
            this.ClearRaces.Location = new System.Drawing.Point(420, 367);
            this.ClearRaces.Name = "ClearRaces";
            this.ClearRaces.Size = new System.Drawing.Size(75, 23);
            this.ClearRaces.TabIndex = 16;
            this.ClearRaces.Text = "Clear";
            this.ClearRaces.UseVisualStyleBackColor = true;
            this.ClearRaces.Click += new System.EventHandler(this.ClearRaces_Click);
            // 
            // ToggleRaces
            // 
            this.ToggleRaces.Location = new System.Drawing.Point(542, 367);
            this.ToggleRaces.Name = "ToggleRaces";
            this.ToggleRaces.Size = new System.Drawing.Size(75, 23);
            this.ToggleRaces.TabIndex = 17;
            this.ToggleRaces.Text = "Toggle";
            this.ToggleRaces.UseVisualStyleBackColor = true;
            this.ToggleRaces.Click += new System.EventHandler(this.ToggleRaces_Click);
            // 
            // ClearRarities
            // 
            this.ClearRarities.Location = new System.Drawing.Point(623, 367);
            this.ClearRarities.Name = "ClearRarities";
            this.ClearRarities.Size = new System.Drawing.Size(75, 23);
            this.ClearRarities.TabIndex = 18;
            this.ClearRarities.Text = "Clear";
            this.ClearRarities.UseVisualStyleBackColor = true;
            this.ClearRarities.Click += new System.EventHandler(this.ClearRarities_Click);
            // 
            // ToggleRarities
            // 
            this.ToggleRarities.Location = new System.Drawing.Point(745, 367);
            this.ToggleRarities.Name = "ToggleRarities";
            this.ToggleRarities.Size = new System.Drawing.Size(75, 23);
            this.ToggleRarities.TabIndex = 19;
            this.ToggleRarities.Text = "Toggle";
            this.ToggleRarities.UseVisualStyleBackColor = true;
            this.ToggleRarities.Click += new System.EventHandler(this.ToggleRarities_Click);
            // 
            // ClearExpansions
            // 
            this.ClearExpansions.Location = new System.Drawing.Point(826, 367);
            this.ClearExpansions.Name = "ClearExpansions";
            this.ClearExpansions.Size = new System.Drawing.Size(75, 23);
            this.ClearExpansions.TabIndex = 20;
            this.ClearExpansions.Text = "Clear";
            this.ClearExpansions.UseVisualStyleBackColor = true;
            this.ClearExpansions.Click += new System.EventHandler(this.ClearExpansions_Click);
            // 
            // ToggleExpansions
            // 
            this.ToggleExpansions.Location = new System.Drawing.Point(948, 367);
            this.ToggleExpansions.Name = "ToggleExpansions";
            this.ToggleExpansions.Size = new System.Drawing.Size(75, 23);
            this.ToggleExpansions.TabIndex = 21;
            this.ToggleExpansions.Text = "Toggle";
            this.ToggleExpansions.UseVisualStyleBackColor = true;
            this.ToggleExpansions.Click += new System.EventHandler(this.ToggleExpansions_Click);
            // 
            // QuickFilterForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(1036, 437);
            this.Controls.Add(this.ToggleExpansions);
            this.Controls.Add(this.ClearExpansions);
            this.Controls.Add(this.ToggleRarities);
            this.Controls.Add(this.ClearRarities);
            this.Controls.Add(this.ToggleRaces);
            this.Controls.Add(this.ClearRaces);
            this.Controls.Add(this.ToggleClasses);
            this.Controls.Add(this.ClearClasses);
            this.Controls.Add(this.ToggleFactions);
            this.Controls.Add(this.ClearFactions);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ListExpansions);
            this.Controls.Add(this.ListRarities);
            this.Controls.Add(this.ListRaces);
            this.Controls.Add(this.ListClasses);
            this.Controls.Add(this.ListFactions);
            this.Name = "QuickFilterForm";
            this.Text = "Quick filter";
            this.Load += new System.EventHandler(this.QuickFilterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox ListFactions;
        private System.Windows.Forms.CheckedListBox ListClasses;
        private System.Windows.Forms.CheckedListBox ListRaces;
        private System.Windows.Forms.CheckedListBox ListRarities;
        private System.Windows.Forms.CheckedListBox ListExpansions;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ClearFactions;
        private System.Windows.Forms.Button ToggleFactions;
        private System.Windows.Forms.Button ClearClasses;
        private System.Windows.Forms.Button ToggleClasses;
        private System.Windows.Forms.Button ClearRaces;
        private System.Windows.Forms.Button ToggleRaces;
        private System.Windows.Forms.Button ClearRarities;
        private System.Windows.Forms.Button ToggleRarities;
        private System.Windows.Forms.Button ClearExpansions;
        private System.Windows.Forms.Button ToggleExpansions;
    }
}