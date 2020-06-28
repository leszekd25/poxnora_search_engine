namespace poxnora_search_engine.Pox.FilterControls
{
    partial class EnumFilterControl
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
            this.FilterValue = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.Label();
            this.FilterType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // FilterValue
            // 
            this.FilterValue.FormattingEnabled = true;
            this.FilterValue.Location = new System.Drawing.Point(189, 53);
            this.FilterValue.Name = "FilterValue";
            this.FilterValue.Size = new System.Drawing.Size(134, 21);
            this.FilterValue.TabIndex = 13;
            this.FilterValue.SelectedIndexChanged += new System.EventHandler(this.FilterValue_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Value";
            // 
            // textBox1
            // 
            this.textBox1.AutoSize = true;
            this.textBox1.Location = new System.Drawing.Point(3, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(52, 13);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "Filter type";
            // 
            // FilterType
            // 
            this.FilterType.FormattingEnabled = true;
            this.FilterType.Location = new System.Drawing.Point(189, 26);
            this.FilterType.Name = "FilterType";
            this.FilterType.Size = new System.Drawing.Size(134, 21);
            this.FilterType.TabIndex = 10;
            this.FilterType.SelectedIndexChanged += new System.EventHandler(this.FilterType_SelectedIndexChanged);
            // 
            // EnumFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.FilterValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.FilterType);
            this.Name = "EnumFilterControl";
            this.Controls.SetChildIndex(this.FilterType, 0);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.FilterValue, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox FilterValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label textBox1;
        private System.Windows.Forms.ComboBox FilterType;
    }
}
