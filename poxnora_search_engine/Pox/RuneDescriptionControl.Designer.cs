namespace poxnora_search_engine.Pox
{
    partial class RuneDescriptionControl
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
            this.RuneImage = new System.Windows.Forms.PictureBox();
            this.TextBoxDescription = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.RuneImage)).BeginInit();
            this.SuspendLayout();
            // 
            // RuneImage
            // 
            this.RuneImage.Location = new System.Drawing.Point(3, 3);
            this.RuneImage.Name = "RuneImage";
            this.RuneImage.Size = new System.Drawing.Size(275, 311);
            this.RuneImage.TabIndex = 0;
            this.RuneImage.TabStop = false;
            // 
            // TextBoxDescription
            // 
            this.TextBoxDescription.BackColor = System.Drawing.Color.Black;
            this.TextBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBoxDescription.ForeColor = System.Drawing.Color.LightGray;
            this.TextBoxDescription.Location = new System.Drawing.Point(3, 320);
            this.TextBoxDescription.Name = "TextBoxDescription";
            this.TextBoxDescription.Size = new System.Drawing.Size(275, 255);
            this.TextBoxDescription.TabIndex = 1;
            this.TextBoxDescription.Text = "";
            // 
            // RuneDescriptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.TextBoxDescription);
            this.Controls.Add(this.RuneImage);
            this.Name = "RuneDescriptionControl";
            this.Size = new System.Drawing.Size(281, 578);
            ((System.ComponentModel.ISupportInitialize)(this.RuneImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox RuneImage;
        private System.Windows.Forms.RichTextBox TextBoxDescription;
    }
}
