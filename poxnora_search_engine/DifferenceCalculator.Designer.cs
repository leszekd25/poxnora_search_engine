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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Champions");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Abiilities");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Spells");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Relics");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Equipments");
            this.LoadOldDatabaseDialog = new System.Windows.Forms.OpenFileDialog();
            this.ChangesTree = new System.Windows.Forms.TreeView();
            this.ChangeListModeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showChangesPerCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showChangesPerFactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelChangeList = new System.Windows.Forms.Panel();
            this.ButtonPrevious = new System.Windows.Forms.Button();
            this.ButtonCurrent = new System.Windows.Forms.Button();
            this.RuneDescription = new poxnora_search_engine.Pox.RuneDescriptionControl();
            this.DatabaseFilter = new poxnora_search_engine.Pox.DatabaseFilterControl();
            this.ChangeListModeMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadOldDatabaseDialog
            // 
            this.LoadOldDatabaseDialog.Filter = "JSON|*.json";
            // 
            // ChangesTree
            // 
            this.ChangesTree.ContextMenuStrip = this.ChangeListModeMenu;
            this.ChangesTree.Location = new System.Drawing.Point(12, 12);
            this.ChangesTree.Name = "ChangesTree";
            treeNode1.Name = "Champions";
            treeNode1.Text = "Champions";
            treeNode2.Name = "Abilities";
            treeNode2.Text = "Abiilities";
            treeNode3.Name = "Spells";
            treeNode3.Text = "Spells";
            treeNode4.Name = "Relics";
            treeNode4.Text = "Relics";
            treeNode5.Name = "Equipments";
            treeNode5.Text = "Equipments";
            this.ChangesTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            this.ChangesTree.Size = new System.Drawing.Size(329, 165);
            this.ChangesTree.TabIndex = 0;
            this.ChangesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ChangesTree_AfterSelect);
            // 
            // ChangeListModeMenu
            // 
            this.ChangeListModeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showChangesPerCategoryToolStripMenuItem,
            this.showChangesPerFactionToolStripMenuItem});
            this.ChangeListModeMenu.Name = "ChangeListModeMenu";
            this.ChangeListModeMenu.Size = new System.Drawing.Size(220, 48);
            // 
            // showChangesPerCategoryToolStripMenuItem
            // 
            this.showChangesPerCategoryToolStripMenuItem.Name = "showChangesPerCategoryToolStripMenuItem";
            this.showChangesPerCategoryToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.showChangesPerCategoryToolStripMenuItem.Text = "Show changes per category";
            this.showChangesPerCategoryToolStripMenuItem.Click += new System.EventHandler(this.showChangesPerCategoryToolStripMenuItem_Click);
            // 
            // showChangesPerFactionToolStripMenuItem
            // 
            this.showChangesPerFactionToolStripMenuItem.Name = "showChangesPerFactionToolStripMenuItem";
            this.showChangesPerFactionToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.showChangesPerFactionToolStripMenuItem.Text = "Show changes per faction";
            this.showChangesPerFactionToolStripMenuItem.Click += new System.EventHandler(this.showChangesPerFactionToolStripMenuItem_Click);
            // 
            // PanelChangeList
            // 
            this.PanelChangeList.AutoScroll = true;
            this.PanelChangeList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelChangeList.Location = new System.Drawing.Point(347, 12);
            this.PanelChangeList.Name = "PanelChangeList";
            this.PanelChangeList.Size = new System.Drawing.Size(447, 431);
            this.PanelChangeList.TabIndex = 2;
            // 
            // ButtonPrevious
            // 
            this.ButtonPrevious.Location = new System.Drawing.Point(347, 449);
            this.ButtonPrevious.Name = "ButtonPrevious";
            this.ButtonPrevious.Size = new System.Drawing.Size(179, 23);
            this.ButtonPrevious.TabIndex = 3;
            this.ButtonPrevious.Text = "Previous";
            this.ButtonPrevious.UseVisualStyleBackColor = true;
            this.ButtonPrevious.Click += new System.EventHandler(this.ButtonPrevious_Click);
            // 
            // ButtonCurrent
            // 
            this.ButtonCurrent.Location = new System.Drawing.Point(615, 449);
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
            this.RuneDescription.Location = new System.Drawing.Point(800, 10);
            this.RuneDescription.Name = "RuneDescription";
            this.RuneDescription.Size = new System.Drawing.Size(281, 578);
            this.RuneDescription.TabIndex = 1;
            // 
            // DatabaseFilter
            // 
            this.DatabaseFilter.Location = new System.Drawing.Point(9, 183);
            this.DatabaseFilter.Name = "DatabaseFilter";
            this.DatabaseFilter.Size = new System.Drawing.Size(334, 405);
            this.DatabaseFilter.TabIndex = 5;
            // 
            // DifferenceCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 600);
            this.Controls.Add(this.DatabaseFilter);
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
            this.ChangeListModeMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog LoadOldDatabaseDialog;
        private System.Windows.Forms.TreeView ChangesTree;
        private Pox.RuneDescriptionControl RuneDescription;
        private System.Windows.Forms.Panel PanelChangeList;
        private System.Windows.Forms.Button ButtonPrevious;
        private System.Windows.Forms.Button ButtonCurrent;
        private System.Windows.Forms.ContextMenuStrip ChangeListModeMenu;
        private System.Windows.Forms.ToolStripMenuItem showChangesPerCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showChangesPerFactionToolStripMenuItem;
        private Pox.DatabaseFilterControl DatabaseFilter;
    }
}