namespace poxnora_search_engine
{
    partial class AboutBox
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
            this.label1 = new System.Windows.Forms.Label();
            this.LabelCurrentVersion = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LinkGithub = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "PoxNora Search Engine";
            // 
            // LabelCurrentVersion
            // 
            this.LabelCurrentVersion.AutoSize = true;
            this.LabelCurrentVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelCurrentVersion.Location = new System.Drawing.Point(16, 113);
            this.LabelCurrentVersion.Name = "LabelCurrentVersion";
            this.LabelCurrentVersion.Size = new System.Drawing.Size(84, 13);
            this.LabelCurrentVersion.TabIndex = 1;
            this.LabelCurrentVersion.Text = "Current version: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(211, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Link to GitHub repository of this application:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Contact me on Discord: shovel_knight";
            // 
            // LinkGithub
            // 
            this.LinkGithub.AutoSize = true;
            this.LinkGithub.Location = new System.Drawing.Point(223, 66);
            this.LinkGithub.Name = "LinkGithub";
            this.LinkGithub.Size = new System.Drawing.Size(28, 13);
            this.LinkGithub.TabIndex = 4;
            this.LinkGithub.TabStop = true;
            this.LinkGithub.Text = "here";
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 137);
            this.Controls.Add(this.LinkGithub);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LabelCurrentVersion);
            this.Controls.Add(this.label1);
            this.Name = "AboutBox";
            this.Text = "About";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AboutBox_FormClosed);
            this.Load += new System.EventHandler(this.AboutBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabelCurrentVersion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel LinkGithub;
    }
}