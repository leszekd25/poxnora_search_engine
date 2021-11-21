
namespace poxnora_search_engine
{
    partial class BGBSaveForm
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
            this.ListboxExistingBGs = new System.Windows.Forms.ListBox();
            this.TextboxBGName = new System.Windows.Forms.TextBox();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListboxExistingBGs
            // 
            this.ListboxExistingBGs.FormattingEnabled = true;
            this.ListboxExistingBGs.Location = new System.Drawing.Point(8, 8);
            this.ListboxExistingBGs.Name = "ListboxExistingBGs";
            this.ListboxExistingBGs.Size = new System.Drawing.Size(288, 329);
            this.ListboxExistingBGs.TabIndex = 1;
            this.ListboxExistingBGs.SelectedIndexChanged += new System.EventHandler(this.ListboxExistingBGs_SelectedIndexChanged);
            // 
            // TextboxBGName
            // 
            this.TextboxBGName.Location = new System.Drawing.Point(8, 343);
            this.TextboxBGName.Name = "TextboxBGName";
            this.TextboxBGName.Size = new System.Drawing.Size(288, 20);
            this.TextboxBGName.TabIndex = 2;
            // 
            // ButtonSave
            // 
            this.ButtonSave.Location = new System.Drawing.Point(8, 404);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(288, 23);
            this.ButtonSave.TabIndex = 3;
            this.ButtonSave.Text = "Save";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // BGBSaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 434);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.TextboxBGName);
            this.Controls.Add(this.ListboxExistingBGs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BGBSaveForm";
            this.Text = "Choose save name";
            this.Load += new System.EventHandler(this.BGBSaveForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListboxExistingBGs;
        private System.Windows.Forms.TextBox TextboxBGName;
        private System.Windows.Forms.Button ButtonSave;
    }
}