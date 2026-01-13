namespace WindowsForms.UserControls
{
    partial class PlayerUserControl
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
            pictureBoxPlayer = new System.Windows.Forms.PictureBox();
            labelPlayerName = new System.Windows.Forms.Label();
            labelPosition = new System.Windows.Forms.Label();
            labelStar = new System.Windows.Forms.Label();
            panelInfo = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPlayer).BeginInit();
            panelInfo.SuspendLayout();
            SuspendLayout();
            //
            // pictureBoxPlayer
            //
            pictureBoxPlayer.BackColor = System.Drawing.Color.LightGray;
            pictureBoxPlayer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pictureBoxPlayer.Location = new System.Drawing.Point(5, 10);
            pictureBoxPlayer.Name = "pictureBoxPlayer";
            pictureBoxPlayer.Size = new System.Drawing.Size(50, 50);
            pictureBoxPlayer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBoxPlayer.TabIndex = 0;
            pictureBoxPlayer.TabStop = false;
            //
            // labelPlayerName
            //
            labelPlayerName.AutoSize = true;
            labelPlayerName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            labelPlayerName.Location = new System.Drawing.Point(3, 5);
            labelPlayerName.Name = "labelPlayerName";
            labelPlayerName.Size = new System.Drawing.Size(120, 17);
            labelPlayerName.TabIndex = 1;
            labelPlayerName.Text = "#10 Player Name";
            //
            // labelPosition
            //
            labelPosition.AutoSize = true;
            labelPosition.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelPosition.ForeColor = System.Drawing.Color.DimGray;
            labelPosition.Location = new System.Drawing.Point(3, 25);
            labelPosition.Name = "labelPosition";
            labelPosition.Size = new System.Drawing.Size(52, 15);
            labelPosition.TabIndex = 2;
            labelPosition.Text = "Position";
            //
            // labelStar
            //
            labelStar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelStar.Cursor = System.Windows.Forms.Cursors.Hand;
            labelStar.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelStar.ForeColor = System.Drawing.Color.LightGray;
            labelStar.Location = new System.Drawing.Point(177, 3);
            labelStar.Name = "labelStar";
            labelStar.Size = new System.Drawing.Size(25, 25);
            labelStar.TabIndex = 3;
            labelStar.Text = "\u2606";
            labelStar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // panelInfo
            //
            panelInfo.Controls.Add(labelPlayerName);
            panelInfo.Controls.Add(labelPosition);
            panelInfo.Location = new System.Drawing.Point(60, 10);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new System.Drawing.Size(115, 50);
            panelInfo.TabIndex = 4;
            //
            // PlayerUserControl
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(labelStar);
            Controls.Add(panelInfo);
            Controls.Add(pictureBoxPlayer);
            Cursor = System.Windows.Forms.Cursors.Hand;
            MinimumSize = new System.Drawing.Size(200, 70);
            Name = "PlayerUserControl";
            Size = new System.Drawing.Size(205, 70);
            ((System.ComponentModel.ISupportInitialize)pictureBoxPlayer).EndInit();
            panelInfo.ResumeLayout(false);
            panelInfo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPlayer;
        private System.Windows.Forms.Label labelPlayerName;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.Label labelStar;
        private System.Windows.Forms.Panel panelInfo;
    }
}
