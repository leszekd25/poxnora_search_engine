﻿namespace poxnora_search_engine.Pox.FilterControls
{
    partial class IntFilterControl
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
            this.FilterType = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // FilterType
            // 
            this.FilterType.FormattingEnabled = true;
            this.FilterType.Location = new System.Drawing.Point(189, 26);
            this.FilterType.Name = "FilterType";
            this.FilterType.Size = new System.Drawing.Size(134, 21);
            this.FilterType.TabIndex = 2;
            this.FilterType.SelectedIndexChanged += new System.EventHandler(this.FilterType_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.AutoSize = true;
            this.textBox1.Location = new System.Drawing.Point(3, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(52, 13);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "Filter type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Value";
            // 
            // FilterValue
            // 
            this.FilterValue.Location = new System.Drawing.Point(189, 53);
            this.FilterValue.Name = "FilterValue";
            this.FilterValue.Size = new System.Drawing.Size(134, 20);
            this.FilterValue.TabIndex = 5;
            this.FilterValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterValue_KeyDown);
            this.FilterValue.Validated += new System.EventHandler(this.FilterValue_Validated);
            // 
            // IntFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.FilterValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.FilterType);
            this.Name = "IntFilterControl";
            this.Controls.SetChildIndex(this.FilterType, 0);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.FilterValue, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox FilterType;
        private System.Windows.Forms.Label textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FilterValue;
    }
}
