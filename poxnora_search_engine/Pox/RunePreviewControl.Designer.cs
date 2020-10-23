namespace poxnora_search_engine.Pox
{
    partial class RunePreviewControl
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
            this.RunePreviewImage = new System.Windows.Forms.PictureBox();
            this.LabelText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.RunePreviewImage)).BeginInit();
            this.SuspendLayout();
            // 
            // RunePreviewImage
            // 
            this.RunePreviewImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RunePreviewImage.Location = new System.Drawing.Point(15, 3);
            this.RunePreviewImage.Name = "RunePreviewImage";
            this.RunePreviewImage.Size = new System.Drawing.Size(46, 62);
            this.RunePreviewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RunePreviewImage.TabIndex = 0;
            this.RunePreviewImage.TabStop = false;
            // 
            // LabelText
            // 
            this.LabelText.Location = new System.Drawing.Point(3, 68);
            this.LabelText.Name = "LabelText";
            this.LabelText.Size = new System.Drawing.Size(70, 35);
            this.LabelText.TabIndex = 1;
            this.LabelText.Text = "rune";
            this.LabelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RunePreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LabelText);
            this.Controls.Add(this.RunePreviewImage);
            this.Name = "RunePreviewControl";
            this.Size = new System.Drawing.Size(76, 107);
            this.Resize += new System.EventHandler(this.RunePreviewControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.RunePreviewImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox RunePreviewImage;
        public System.Windows.Forms.Label LabelText;
    }
}
