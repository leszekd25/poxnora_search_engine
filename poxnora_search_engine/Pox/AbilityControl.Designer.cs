
namespace poxnora_search_engine.Pox
{
    partial class AbilityControl
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
            this.LabelAbilityNoraCost = new System.Windows.Forms.Label();
            this.PictureBoxAbility = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbility)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelAbilityName
            // 
            this.LabelAbilityName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.LabelAbilityName.Location = new System.Drawing.Point(40, 0);
            this.LabelAbilityName.Name = "LabelAbilityName";
            this.LabelAbilityName.Size = new System.Drawing.Size(207, 18);
            this.LabelAbilityName.TabIndex = 1;
            this.LabelAbilityName.Text = "lorem ipsum";
            this.LabelAbilityName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelAbilityNoraCost
            // 
            this.LabelAbilityNoraCost.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.LabelAbilityNoraCost.Location = new System.Drawing.Point(40, 18);
            this.LabelAbilityNoraCost.Name = "LabelAbilityNoraCost";
            this.LabelAbilityNoraCost.Size = new System.Drawing.Size(207, 17);
            this.LabelAbilityNoraCost.TabIndex = 2;
            this.LabelAbilityNoraCost.Text = "7 nora";
            this.LabelAbilityNoraCost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PictureBoxAbility
            // 
            this.PictureBoxAbility.Location = new System.Drawing.Point(0, 0);
            this.PictureBoxAbility.Name = "PictureBoxAbility";
            this.PictureBoxAbility.Size = new System.Drawing.Size(34, 35);
            this.PictureBoxAbility.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBoxAbility.TabIndex = 0;
            this.PictureBoxAbility.TabStop = false;
            // 
            // AbilityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.LabelAbilityNoraCost);
            this.Controls.Add(this.LabelAbilityName);
            this.Controls.Add(this.PictureBoxAbility);
            this.Name = "AbilityControl";
            this.Size = new System.Drawing.Size(250, 35);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxAbility)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label LabelAbilityName;
        private System.Windows.Forms.Label LabelAbilityNoraCost;
        public System.Windows.Forms.PictureBox PictureBoxAbility;
    }
}
