namespace poxnora_search_engine.Pox.FilterControls
{
    partial class BaseFilterControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LabelName = new System.Windows.Forms.Label();
            this.FilterNegated = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Location = new System.Drawing.Point(3, 4);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(57, 13);
            this.LabelName.TabIndex = 0;
            this.LabelName.Text = "FilterName";
            // 
            // FilterNegated
            // 
            this.FilterNegated.AutoSize = true;
            this.FilterNegated.Location = new System.Drawing.Point(234, 3);
            this.FilterNegated.Name = "FilterNegated";
            this.FilterNegated.Size = new System.Drawing.Size(89, 17);
            this.FilterNegated.TabIndex = 1;
            this.FilterNegated.Text = "Negate result";
            this.FilterNegated.UseVisualStyleBackColor = true;
            this.FilterNegated.CheckedChanged += new System.EventHandler(this.FilterNegated_CheckedChanged);
            // 
            // BaseFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Khaki;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.FilterNegated);
            this.Controls.Add(this.LabelName);
            this.Name = "BaseFilterControl";
            this.Size = new System.Drawing.Size(324, 139);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.CheckBox FilterNegated;
    }
}
