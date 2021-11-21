
namespace poxnora_search_engine
{
    partial class BGBLoadForm
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
            this.ListBoxCodeNames = new System.Windows.Forms.ListBox();
            this.ButtonLoadCode = new System.Windows.Forms.Button();
            this.ButtonDeleteCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListBoxCodeNames
            // 
            this.ListBoxCodeNames.FormattingEnabled = true;
            this.ListBoxCodeNames.Location = new System.Drawing.Point(12, 12);
            this.ListBoxCodeNames.Name = "ListBoxCodeNames";
            this.ListBoxCodeNames.Size = new System.Drawing.Size(279, 316);
            this.ListBoxCodeNames.TabIndex = 0;
            // 
            // ButtonLoadCode
            // 
            this.ButtonLoadCode.Location = new System.Drawing.Point(297, 305);
            this.ButtonLoadCode.Name = "ButtonLoadCode";
            this.ButtonLoadCode.Size = new System.Drawing.Size(123, 23);
            this.ButtonLoadCode.TabIndex = 1;
            this.ButtonLoadCode.Text = "Load battlegroup";
            this.ButtonLoadCode.UseVisualStyleBackColor = true;
            this.ButtonLoadCode.Click += new System.EventHandler(this.ButtonLoadCode_Click);
            // 
            // ButtonDeleteCode
            // 
            this.ButtonDeleteCode.Location = new System.Drawing.Point(297, 12);
            this.ButtonDeleteCode.Name = "ButtonDeleteCode";
            this.ButtonDeleteCode.Size = new System.Drawing.Size(123, 23);
            this.ButtonDeleteCode.TabIndex = 2;
            this.ButtonDeleteCode.Text = "Delete battlegroup";
            this.ButtonDeleteCode.UseVisualStyleBackColor = true;
            this.ButtonDeleteCode.Click += new System.EventHandler(this.ButtonDeleteCode_Click);
            // 
            // BGBLoadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 340);
            this.Controls.Add(this.ButtonDeleteCode);
            this.Controls.Add(this.ButtonLoadCode);
            this.Controls.Add(this.ListBoxCodeNames);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BGBLoadForm";
            this.Text = "Select battlegroup to load";
            this.Load += new System.EventHandler(this.BGBLoadForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ListBoxCodeNames;
        private System.Windows.Forms.Button ButtonLoadCode;
        private System.Windows.Forms.Button ButtonDeleteCode;
    }
}