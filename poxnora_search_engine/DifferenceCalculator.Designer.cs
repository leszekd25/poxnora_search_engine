namespace poxnora_search_engine
{
    partial class DifferenceCalculator
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
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Champions");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Abiilities");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Spells");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Relics");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Equipments");
            this.LoadOldDatabaseDialog = new System.Windows.Forms.OpenFileDialog();
            this.ChangesTree = new System.Windows.Forms.TreeView();
            this.PanelChangeList = new System.Windows.Forms.Panel();
            this.ButtonPrevious = new System.Windows.Forms.Button();
            this.ButtonCurrent = new System.Windows.Forms.Button();
            this.RuneDescription = new poxnora_search_engine.Pox.RuneDescriptionControl();
            this.SuspendLayout();
            // 
            // LoadOldDatabaseDialog
            // 
            this.LoadOldDatabaseDialog.Filter = "JSON|*.json";
            // 
            // ChangesTree
            // 
            this.ChangesTree.Location = new System.Drawing.Point(12, 12);
            this.ChangesTree.Name = "ChangesTree";
            treeNode6.Name = "Champions";
            treeNode6.Text = "Champions";
            treeNode7.Name = "Abilities";
            treeNode7.Text = "Abiilities";
            treeNode8.Name = "Spells";
            treeNode8.Text = "Spells";
            treeNode9.Name = "Relics";
            treeNode9.Text = "Relics";
            treeNode10.Name = "Equipments";
            treeNode10.Text = "Equipments";
            this.ChangesTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10});
            this.ChangesTree.Size = new System.Drawing.Size(242, 576);
            this.ChangesTree.TabIndex = 0;
            this.ChangesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChangesTree_AfterSelect);
            // 
            // PanelChangeList
            // 
            this.PanelChangeList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelChangeList.Location = new System.Drawing.Point(260, 12);
            this.PanelChangeList.Name = "PanelChangeList";
            this.PanelChangeList.Size = new System.Drawing.Size(447, 431);
            this.PanelChangeList.TabIndex = 2;
            // 
            // ButtonPrevious
            // 
            this.ButtonPrevious.Location = new System.Drawing.Point(260, 449);
            this.ButtonPrevious.Name = "ButtonPrevious";
            this.ButtonPrevious.Size = new System.Drawing.Size(179, 23);
            this.ButtonPrevious.TabIndex = 3;
            this.ButtonPrevious.Text = "Previous";
            this.ButtonPrevious.UseVisualStyleBackColor = true;
            this.ButtonPrevious.Click += new System.EventHandler(this.ButtonPrevious_Click);
            // 
            // ButtonCurrent
            // 
            this.ButtonCurrent.Location = new System.Drawing.Point(528, 449);
            this.ButtonCurrent.Name = "ButtonCurrent";
            this.ButtonCurrent.Size = new System.Drawing.Size(179, 23);
            this.ButtonCurrent.TabIndex = 4;
            this.ButtonCurrent.Text = "Current";
            this.ButtonCurrent.UseVisualStyleBackColor = true;
            this.ButtonCurrent.Click += new System.EventHandler(this.ButtonCurrent_Click);
            // 
            // RuneDescription
            // 
            this.RuneDescription.BackColor = System.Drawing.Color.Black;
            this.RuneDescription.Location = new System.Drawing.Point(713, 10);
            this.RuneDescription.Name = "RuneDescription";
            this.RuneDescription.Size = new System.Drawing.Size(281, 578);
            this.RuneDescription.TabIndex = 1;
            // 
            // DifferenceCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 600);
            this.Controls.Add(this.ButtonCurrent);
            this.Controls.Add(this.ButtonPrevious);
            this.Controls.Add(this.PanelChangeList);
            this.Controls.Add(this.RuneDescription);
            this.Controls.Add(this.ChangesTree);
            this.Name = "DifferenceCalculator";
            this.Text = "DifferenceCalculatorForm";
            this.Activated += new System.EventHandler(this.DifferenceCalculator_Activated);
            this.Deactivate += new System.EventHandler(this.DifferenceCalculator_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DifferenceCalculator_FormClosed);
            this.Load += new System.EventHandler(this.DifferenceCalculatorForm_Load);
            this.Resize += new System.EventHandler(this.DifferenceCalculatorForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog LoadOldDatabaseDialog;
        private System.Windows.Forms.TreeView ChangesTree;
        private Pox.RuneDescriptionControl RuneDescription;
        private System.Windows.Forms.Panel PanelChangeList;
        private System.Windows.Forms.Button ButtonPrevious;
        private System.Windows.Forms.Button ButtonCurrent;
    }
}