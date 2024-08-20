
namespace poxnora_search_engine
{
    partial class PatchBuilderSaveForm
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
            this.ButtonSave = new System.Windows.Forms.Button();
            this.TextboxPatchName = new System.Windows.Forms.TextBox();
            this.ListboxExistingPatches = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ButtonSave
            // 
            this.ButtonSave.Location = new System.Drawing.Point(8, 274);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(288, 23);
            this.ButtonSave.TabIndex = 6;
            this.ButtonSave.Text = "Save";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // TextboxPatchName
            // 
            this.TextboxPatchName.Location = new System.Drawing.Point(8, 213);
            this.TextboxPatchName.Name = "TextboxPatchName";
            this.TextboxPatchName.Size = new System.Drawing.Size(288, 20);
            this.TextboxPatchName.TabIndex = 5;
            // 
            // ListboxExistingPatches
            // 
            this.ListboxExistingPatches.FormattingEnabled = true;
            this.ListboxExistingPatches.Location = new System.Drawing.Point(10, 8);
            this.ListboxExistingPatches.Name = "ListboxExistingPatches";
            this.ListboxExistingPatches.Size = new System.Drawing.Size(288, 199);
            this.ListboxExistingPatches.TabIndex = 4;
            this.ListboxExistingPatches.SelectedIndexChanged += new System.EventHandler(this.ListboxExistingPatches_SelectedIndexChanged);
            // 
            // PatchBuilderSaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 303);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.TextboxPatchName);
            this.Controls.Add(this.ListboxExistingPatches);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PatchBuilderSaveForm";
            this.Text = "Ssave patch";
            this.Load += new System.EventHandler(this.PatchBuilderSaveForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonSave;
        private System.Windows.Forms.TextBox TextboxPatchName;
        private System.Windows.Forms.ListBox ListboxExistingPatches;
    }
}