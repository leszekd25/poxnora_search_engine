
namespace poxnora_search_engine.Pox
{
    partial class AbilityControl_Histogram
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
            this.LabelAbilityName = new System.Windows.Forms.Label();
            this.LabelAbilityNum = new System.Windows.Forms.Label();
            this.PictureBoxAbility = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbility)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelAbilityName
            // 
            this.LabelAbilityName.Location = new System.Drawing.Point(74, 0);
            this.LabelAbilityName.Name = "LabelAbilityName";
            this.LabelAbilityName.Size = new System.Drawing.Size(106, 35);
            this.LabelAbilityName.TabIndex = 1;
            this.LabelAbilityName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelAbilityNum
            // 
            this.LabelAbilityNum.Location = new System.Drawing.Point(3, 0);
            this.LabelAbilityNum.Name = "LabelAbilityNum";
            this.LabelAbilityNum.Size = new System.Drawing.Size(25, 35);
            this.LabelAbilityNum.TabIndex = 2;
            this.LabelAbilityNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PictureBoxAbility
            // 
            this.PictureBoxAbility.Location = new System.Drawing.Point(34, 0);
            this.PictureBoxAbility.Name = "PictureBoxAbility";
            this.PictureBoxAbility.Size = new System.Drawing.Size(34, 35);
            this.PictureBoxAbility.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBoxAbility.TabIndex = 0;
            this.PictureBoxAbility.TabStop = false;
            // 
            // AbilityControl_Histogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LabelAbilityNum);
            this.Controls.Add(this.LabelAbilityName);
            this.Controls.Add(this.PictureBoxAbility);
            this.Name = "AbilityControl_Histogram";
            this.Size = new System.Drawing.Size(183, 35);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbility)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox PictureBoxAbility;
        public System.Windows.Forms.Label LabelAbilityName;
        public System.Windows.Forms.Label LabelAbilityNum;
    }
}
