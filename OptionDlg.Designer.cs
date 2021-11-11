namespace PyramidSolitaire
{
    partial class OptionsWin
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
            this.cbShowCards = new System.Windows.Forms.CheckBox();
            this.cbImage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbBack = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbShowHint = new System.Windows.Forms.CheckBox();
            this.cbFlipPile = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbBack)).BeginInit();
            this.SuspendLayout();
            // 
            // cbShowCards
            // 
            this.cbShowCards.AutoSize = true;
            this.cbShowCards.Location = new System.Drawing.Point(16, 39);
            this.cbShowCards.Margin = new System.Windows.Forms.Padding(4);
            this.cbShowCards.Name = "cbShowCards";
            this.cbShowCards.Size = new System.Drawing.Size(175, 21);
            this.cbShowCards.TabIndex = 1;
            this.cbShowCards.Text = "&Show Cards in Pyramid";
            this.cbShowCards.UseVisualStyleBackColor = true;
            this.cbShowCards.CheckedChanged += new System.EventHandler(this.cbShowCards_CheckedChanged);
            // 
            // cbImage
            // 
            this.cbImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImage.FormattingEnabled = true;
            this.cbImage.Location = new System.Drawing.Point(19, 121);
            this.cbImage.Margin = new System.Windows.Forms.Padding(4);
            this.cbImage.Name = "cbImage";
            this.cbImage.Size = new System.Drawing.Size(185, 24);
            this.cbImage.TabIndex = 5;
            this.cbImage.SelectedIndexChanged += new System.EventHandler(this.cbImage_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 100);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Card &Back Image";
            // 
            // pbBack
            // 
            this.pbBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbBack.Location = new System.Drawing.Point(241, 90);
            this.pbBack.Margin = new System.Windows.Forms.Padding(4);
            this.pbBack.Name = "pbBack";
            this.pbBack.Size = new System.Drawing.Size(97, 120);
            this.pbBack.TabIndex = 3;
            this.pbBack.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(16, 220);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 28);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(124, 220);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(295, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Some options will take affect next \'New Game\'";
            // 
            // cbShowHint
            // 
            this.cbShowHint.AutoSize = true;
            this.cbShowHint.Location = new System.Drawing.Point(203, 39);
            this.cbShowHint.Margin = new System.Windows.Forms.Padding(4);
            this.cbShowHint.Name = "cbShowHint";
            this.cbShowHint.Size = new System.Drawing.Size(124, 21);
            this.cbShowHint.TabIndex = 2;
            this.cbShowHint.Text = "Show &Play Hint";
            this.cbShowHint.UseVisualStyleBackColor = true;
            this.cbShowHint.CheckedChanged += new System.EventHandler(this.cbShowHint_CheckedChanged);
            // 
            // cbFlipPile
            // 
            this.cbFlipPile.AutoSize = true;
            this.cbFlipPile.Location = new System.Drawing.Point(16, 67);
            this.cbFlipPile.Name = "cbFlipPile";
            this.cbFlipPile.Size = new System.Drawing.Size(205, 26);
            this.cbFlipPile.TabIndex = 3;
            this.cbFlipPile.Text = "&Flip (reset) Stock Pile";
            this.cbFlipPile.UseVisualStyleBackColor = true;
            this.cbFlipPile.CheckedChanged += new System.EventHandler(this.cbFlipPile_CheckedChanged);
            // 
            // OptionsWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 266);
            this.Controls.Add(this.cbFlipPile);
            this.Controls.Add(this.cbShowHint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pbBack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbImage);
            this.Controls.Add(this.cbShowCards);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pyramid Solitaire Options";
            this.Load += new System.EventHandler(this.OptionsWin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbBack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbShowCards;
        private System.Windows.Forms.ComboBox cbImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbBack;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbShowHint;
        private System.Windows.Forms.CheckBox cbFlipPile;
    }
}