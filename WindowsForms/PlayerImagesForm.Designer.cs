namespace WindowsForms
{
    partial class PlayerImagesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerImagesForm));
            splitContainer = new SplitContainer();
            flowLayoutPlayers = new FlowLayoutPanel();
            labelPlayerList = new Label();
            panel1 = new Panel();
            buttonRemoveImage = new Button();
            buttonFetchFromApi = new Button();
            buttonBrowseImage = new Button();
            labelPlayerInfo = new Label();
            pictureBoxPlayer = new PictureBox();
            labelInstruction = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPlayer).BeginInit();
            SuspendLayout();
            //
            // splitContainer
            //
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            //
            // splitContainer.Panel1
            //
            splitContainer.Panel1.Controls.Add(flowLayoutPlayers);
            splitContainer.Panel1.Controls.Add(labelPlayerList);
            //
            // splitContainer.Panel2
            //
            splitContainer.Panel2.Controls.Add(panel1);
            splitContainer.Panel2.Controls.Add(labelPlayerInfo);
            splitContainer.Panel2.Controls.Add(pictureBoxPlayer);
            splitContainer.Panel2.Controls.Add(labelInstruction);
            splitContainer.Size = new Size(800, 500);
            splitContainer.SplitterDistance = 280;
            splitContainer.TabIndex = 0;
            //
            // flowLayoutPlayers
            //
            flowLayoutPlayers.AutoScroll = true;
            flowLayoutPlayers.Dock = DockStyle.Fill;
            flowLayoutPlayers.FlowDirection = FlowDirection.TopDown;
            flowLayoutPlayers.WrapContents = false;
            flowLayoutPlayers.Location = new Point(0, 31);
            flowLayoutPlayers.Name = "flowLayoutPlayers";
            flowLayoutPlayers.Size = new Size(280, 469);
            flowLayoutPlayers.TabIndex = 1;
            //
            // labelPlayerList
            //
            labelPlayerList.AutoSize = true;
            labelPlayerList.Dock = DockStyle.Top;
            labelPlayerList.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelPlayerList.Location = new Point(0, 0);
            labelPlayerList.Name = "labelPlayerList";
            labelPlayerList.Padding = new Padding(5, 5, 5, 5);
            labelPlayerList.Size = new Size(88, 31);
            labelPlayerList.TabIndex = 0;
            labelPlayerList.Text = "Players";
            //
            // panel1
            //
            panel1.Controls.Add(buttonFetchFromApi);
            panel1.Controls.Add(buttonRemoveImage);
            panel1.Controls.Add(buttonBrowseImage);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 440);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(10);
            panel1.Size = new Size(530, 60);
            panel1.TabIndex = 3;
            //
            // buttonRemoveImage
            //
            buttonRemoveImage.Enabled = false;
            buttonRemoveImage.Location = new Point(170, 13);
            buttonRemoveImage.Name = "buttonRemoveImage";
            buttonRemoveImage.Size = new Size(150, 35);
            buttonRemoveImage.TabIndex = 1;
            buttonRemoveImage.Text = "Remove Image";
            buttonRemoveImage.UseVisualStyleBackColor = true;
            buttonRemoveImage.Click += ButtonRemoveImage_Click;
            //
            // buttonFetchFromApi
            //
            buttonFetchFromApi.Enabled = false;
            buttonFetchFromApi.Location = new Point(327, 13);
            buttonFetchFromApi.Name = "buttonFetchFromApi";
            buttonFetchFromApi.Size = new Size(150, 35);
            buttonFetchFromApi.TabIndex = 2;
            buttonFetchFromApi.Text = "Fetch from API";
            buttonFetchFromApi.UseVisualStyleBackColor = true;
            buttonFetchFromApi.Click += ButtonFetchFromApi_Click;
            //
            // buttonBrowseImage
            //
            buttonBrowseImage.Enabled = false;
            buttonBrowseImage.Location = new Point(13, 13);
            buttonBrowseImage.Name = "buttonBrowseImage";
            buttonBrowseImage.Size = new Size(150, 35);
            buttonBrowseImage.TabIndex = 0;
            buttonBrowseImage.Text = "Browse Image...";
            buttonBrowseImage.UseVisualStyleBackColor = true;
            buttonBrowseImage.Click += ButtonBrowseImage_Click;
            //
            // labelPlayerInfo
            //
            labelPlayerInfo.AutoSize = true;
            labelPlayerInfo.Dock = DockStyle.Top;
            labelPlayerInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelPlayerInfo.Location = new Point(0, 0);
            labelPlayerInfo.Name = "labelPlayerInfo";
            labelPlayerInfo.Padding = new Padding(10);
            labelPlayerInfo.Size = new Size(227, 39);
            labelPlayerInfo.TabIndex = 2;
            labelPlayerInfo.Text = "Select a player from the list";
            //
            // pictureBoxPlayer
            //
            pictureBoxPlayer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxPlayer.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPlayer.Location = new Point(13, 50);
            pictureBoxPlayer.Name = "pictureBoxPlayer";
            pictureBoxPlayer.Size = new Size(505, 380);
            pictureBoxPlayer.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPlayer.TabIndex = 1;
            pictureBoxPlayer.TabStop = false;
            //
            // labelInstruction
            //
            labelInstruction.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelInstruction.Location = new Point(13, 50);
            labelInstruction.Name = "labelInstruction";
            labelInstruction.Size = new Size(505, 380);
            labelInstruction.TabIndex = 0;
            labelInstruction.Text = "Select a player from the list to assign an image";
            labelInstruction.TextAlign = ContentAlignment.MiddleCenter;
            //
            // PlayerImagesForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 500);
            Controls.Add(splitContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 500);
            Name = "PlayerImagesForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Player Images";
            Load += PlayerImagesForm_Load;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel1.PerformLayout();
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxPlayer).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private FlowLayoutPanel flowLayoutPlayers;
        private Label labelPlayerList;
        private PictureBox pictureBoxPlayer;
        private Label labelInstruction;
        private Label labelPlayerInfo;
        private Panel panel1;
        private Button buttonBrowseImage;
        private Button buttonRemoveImage;
        private Button buttonFetchFromApi;
    }
}
