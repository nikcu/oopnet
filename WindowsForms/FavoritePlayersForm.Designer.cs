namespace WindowsForms
{
    partial class FavoritePlayersForm
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
            splitContainer = new System.Windows.Forms.SplitContainer();
            panelFavouritesContainer = new System.Windows.Forms.Panel();
            panelFavourites = new System.Windows.Forms.FlowLayoutPanel();
            panelFavouritesHeader = new System.Windows.Forms.Panel();
            labelFavouritesCount = new System.Windows.Forms.Label();
            labelFavourites = new System.Windows.Forms.Label();
            panelOthersContainer = new System.Windows.Forms.Panel();
            panelOthers = new System.Windows.Forms.FlowLayoutPanel();
            panelOthersHeader = new System.Windows.Forms.Panel();
            labelOthers = new System.Windows.Forms.Label();
            panelBottom = new System.Windows.Forms.Panel();
            labelStatus = new System.Windows.Forms.Label();
            buttonSave = new System.Windows.Forms.Button();
            labelInstructions = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            panelFavouritesContainer.SuspendLayout();
            panelFavouritesHeader.SuspendLayout();
            panelOthersContainer.SuspendLayout();
            panelOthersHeader.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            //
            // splitContainer
            //
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(0, 0);
            splitContainer.Name = "splitContainer";
            //
            // splitContainer.Panel1
            //
            splitContainer.Panel1.Controls.Add(panelFavouritesContainer);
            splitContainer.Panel1MinSize = 250;
            //
            // splitContainer.Panel2
            //
            splitContainer.Panel2.Controls.Add(panelOthersContainer);
            splitContainer.Panel2MinSize = 250;
            splitContainer.Size = new System.Drawing.Size(884, 511);
            splitContainer.SplitterDistance = 430;
            splitContainer.TabIndex = 0;
            //
            // panelFavouritesContainer
            //
            panelFavouritesContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelFavouritesContainer.Controls.Add(panelFavourites);
            panelFavouritesContainer.Controls.Add(panelFavouritesHeader);
            panelFavouritesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            panelFavouritesContainer.Location = new System.Drawing.Point(0, 0);
            panelFavouritesContainer.Name = "panelFavouritesContainer";
            panelFavouritesContainer.Padding = new System.Windows.Forms.Padding(5);
            panelFavouritesContainer.Size = new System.Drawing.Size(430, 511);
            panelFavouritesContainer.TabIndex = 0;
            //
            // panelFavourites
            //
            panelFavourites.AutoScroll = true;
            panelFavourites.BackColor = System.Drawing.Color.FromArgb(245, 250, 245);
            panelFavourites.Dock = System.Windows.Forms.DockStyle.Fill;
            panelFavourites.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            panelFavourites.Location = new System.Drawing.Point(5, 45);
            panelFavourites.Name = "panelFavourites";
            panelFavourites.Padding = new System.Windows.Forms.Padding(10);
            panelFavourites.Size = new System.Drawing.Size(418, 459);
            panelFavourites.TabIndex = 1;
            panelFavourites.WrapContents = false;
            //
            // panelFavouritesHeader
            //
            panelFavouritesHeader.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            panelFavouritesHeader.Controls.Add(labelFavouritesCount);
            panelFavouritesHeader.Controls.Add(labelFavourites);
            panelFavouritesHeader.Dock = System.Windows.Forms.DockStyle.Top;
            panelFavouritesHeader.Location = new System.Drawing.Point(5, 5);
            panelFavouritesHeader.Name = "panelFavouritesHeader";
            panelFavouritesHeader.Size = new System.Drawing.Size(418, 40);
            panelFavouritesHeader.TabIndex = 0;
            //
            // labelFavouritesCount
            //
            labelFavouritesCount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelFavouritesCount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelFavouritesCount.ForeColor = System.Drawing.Color.White;
            labelFavouritesCount.Location = new System.Drawing.Point(338, 8);
            labelFavouritesCount.Name = "labelFavouritesCount";
            labelFavouritesCount.Size = new System.Drawing.Size(70, 25);
            labelFavouritesCount.TabIndex = 1;
            labelFavouritesCount.Text = "(0/3)";
            labelFavouritesCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelFavourites
            //
            labelFavourites.AutoSize = true;
            labelFavourites.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            labelFavourites.ForeColor = System.Drawing.Color.White;
            labelFavourites.Location = new System.Drawing.Point(10, 8);
            labelFavourites.Name = "labelFavourites";
            labelFavourites.Size = new System.Drawing.Size(138, 21);
            labelFavourites.TabIndex = 0;
            labelFavourites.Text = "Favourite Players";
            //
            // panelOthersContainer
            //
            panelOthersContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelOthersContainer.Controls.Add(panelOthers);
            panelOthersContainer.Controls.Add(panelOthersHeader);
            panelOthersContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            panelOthersContainer.Location = new System.Drawing.Point(0, 0);
            panelOthersContainer.Name = "panelOthersContainer";
            panelOthersContainer.Padding = new System.Windows.Forms.Padding(5);
            panelOthersContainer.Size = new System.Drawing.Size(450, 511);
            panelOthersContainer.TabIndex = 0;
            //
            // panelOthers
            //
            panelOthers.AutoScroll = true;
            panelOthers.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            panelOthers.Dock = System.Windows.Forms.DockStyle.Fill;
            panelOthers.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            panelOthers.Location = new System.Drawing.Point(5, 45);
            panelOthers.Name = "panelOthers";
            panelOthers.Padding = new System.Windows.Forms.Padding(10);
            panelOthers.Size = new System.Drawing.Size(438, 459);
            panelOthers.TabIndex = 1;
            panelOthers.WrapContents = false;
            //
            // panelOthersHeader
            //
            panelOthersHeader.BackColor = System.Drawing.Color.FromArgb(96, 125, 139);
            panelOthersHeader.Controls.Add(labelOthers);
            panelOthersHeader.Dock = System.Windows.Forms.DockStyle.Top;
            panelOthersHeader.Location = new System.Drawing.Point(5, 5);
            panelOthersHeader.Name = "panelOthersHeader";
            panelOthersHeader.Size = new System.Drawing.Size(438, 40);
            panelOthersHeader.TabIndex = 0;
            //
            // labelOthers
            //
            labelOthers.AutoSize = true;
            labelOthers.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            labelOthers.ForeColor = System.Drawing.Color.White;
            labelOthers.Location = new System.Drawing.Point(10, 8);
            labelOthers.Name = "labelOthers";
            labelOthers.Size = new System.Drawing.Size(113, 21);
            labelOthers.TabIndex = 0;
            labelOthers.Text = "Other Players";
            //
            // panelBottom
            //
            panelBottom.Controls.Add(labelStatus);
            panelBottom.Controls.Add(buttonSave);
            panelBottom.Controls.Add(labelInstructions);
            panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelBottom.Location = new System.Drawing.Point(0, 511);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new System.Drawing.Size(884, 50);
            panelBottom.TabIndex = 1;
            //
            // labelStatus
            //
            labelStatus.AutoSize = true;
            labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            labelStatus.ForeColor = System.Drawing.Color.Gray;
            labelStatus.Location = new System.Drawing.Point(12, 28);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new System.Drawing.Size(62, 15);
            labelStatus.TabIndex = 2;
            labelStatus.Text = "Loading...";
            labelStatus.Visible = false;
            //
            // buttonSave
            //
            buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            buttonSave.Location = new System.Drawing.Point(759, 10);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(113, 32);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            //
            // labelInstructions
            //
            labelInstructions.AutoSize = true;
            labelInstructions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelInstructions.ForeColor = System.Drawing.Color.DimGray;
            labelInstructions.Location = new System.Drawing.Point(12, 10);
            labelInstructions.Name = "labelInstructions";
            labelInstructions.Size = new System.Drawing.Size(413, 15);
            labelInstructions.TabIndex = 0;
            labelInstructions.Text = "Drag players between panels, double-click, or use context menu to manage.";
            //
            // FavoritePlayersForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(884, 561);
            Controls.Add(splitContainer);
            Controls.Add(panelBottom);
            MinimumSize = new System.Drawing.Size(700, 500);
            Name = "FavoritePlayersForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Favourite Players";
            FormClosing += FavoritePlayersForm_FormClosing;
            Load += FavoritePlayersForm_Load;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            panelFavouritesContainer.ResumeLayout(false);
            panelFavouritesHeader.ResumeLayout(false);
            panelFavouritesHeader.PerformLayout();
            panelOthersContainer.ResumeLayout(false);
            panelOthersHeader.ResumeLayout(false);
            panelOthersHeader.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panelFavouritesContainer;
        private System.Windows.Forms.FlowLayoutPanel panelFavourites;
        private System.Windows.Forms.Panel panelFavouritesHeader;
        private System.Windows.Forms.Label labelFavouritesCount;
        private System.Windows.Forms.Label labelFavourites;
        private System.Windows.Forms.Panel panelOthersContainer;
        private System.Windows.Forms.FlowLayoutPanel panelOthers;
        private System.Windows.Forms.Panel panelOthersHeader;
        private System.Windows.Forms.Label labelOthers;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelInstructions;
    }
}
