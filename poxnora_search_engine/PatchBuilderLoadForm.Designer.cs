
namespace poxnora_search_engine
{
    partial class PatchBuilderLoadForm
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
            this.ButtonLoadPatch = new System.Windows.Forms.Button();
            this.ListPatches = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ButtonLoadPatch
            // 
            this.ButtonLoadPatch.Location = new System.Drawing.Point(297, 305);
            this.ButtonLoadPatch.Name = "ButtonLoadPatch";
            this.ButtonLoadPatch.Size = new System.Drawing.Size(78, 23);
            this.ButtonLoadPatch.TabIndex = 4;
            this.ButtonLoadPatch.Text = "Load";
            this.ButtonLoadPatch.UseVisualStyleBackColor = true;
            this.ButtonLoadPatch.Click += new System.EventHandler(this.ButtonLoadPatch_Click);
            // 
            // ListPatches
            // 
            this.ListPatches.FormattingEnabled = true;
            this.ListPatches.Location = new System.Drawing.Point(12, 12);
            this.ListPatches.Name = "ListPatches";
            this.ListPatches.Size = new System.Drawing.Size(279, 316);
            this.ListPatches.TabIndex = 3;
            // 
            // PatchBuilderLoadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 340);
            this.Controls.Add(this.ButtonLoadPatch);
            this.Controls.Add(this.ListPatches);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PatchBuilderLoadForm";
            this.Text = "Load patch";
            this.Load += new System.EventHandler(this.PatchBuilderLoadForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ButtonLoadPatch;
        private System.Windows.Forms.ListBox ListPatches;
    }
}