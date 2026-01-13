using Utils;

namespace WindowsForms
{
    partial class SettingsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            flowLayoutPanel1 = new FlowLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            labelLanguage = new Label();
            comboBoxLanguage = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            labelChampionship = new Label();
            comboBoxChampionship = new ComboBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            labelFavouriteTeam = new Label();
            comboBoxFavouriteTeam = new ComboBox();
            buttonSave = new Button();
            buttonTestApi = new Button();
            buttonViewRankings = new Button();
            buttonPlayerImages = new Button();
            buttonFavouritePlayers = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(tableLayoutPanel2);
            flowLayoutPanel1.Controls.Add(tableLayoutPanel1);
            flowLayoutPanel1.Controls.Add(tableLayoutPanel4);
            flowLayoutPanel1.Controls.Add(buttonSave);
            flowLayoutPanel1.Controls.Add(buttonTestApi);
            flowLayoutPanel1.Controls.Add(buttonViewRankings);
            flowLayoutPanel1.Controls.Add(buttonPlayerImages);
            flowLayoutPanel1.Controls.Add(buttonFavouritePlayers);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(292, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(254, 435);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(labelLanguage, 0, 0);
            tableLayoutPanel2.Controls.Add(comboBoxLanguage, 1, 0);
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(254, 29);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // labelLanguage
            // 
            labelLanguage.Anchor = AnchorStyles.Left;
            labelLanguage.AutoSize = true;
            labelLanguage.Location = new Point(3, 7);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new Size(84, 15);
            labelLanguage.TabIndex = 0;
            labelLanguage.Text = "labelLanguage";
            // 
            // comboBoxLanguage
            // 
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Location = new Point(130, 3);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new Size(121, 23);
            comboBoxLanguage.TabIndex = 1;
            comboBoxLanguage.SelectionChangeCommitted += ComboBoxLanguage_SelectionChangeCommitted;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(labelChampionship, 0, 0);
            tableLayoutPanel1.Controls.Add(comboBoxChampionship, 1, 0);
            tableLayoutPanel1.Location = new Point(0, 29);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(254, 29);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // labelChampionship
            // 
            labelChampionship.Anchor = AnchorStyles.Left;
            labelChampionship.AutoSize = true;
            labelChampionship.Location = new Point(3, 7);
            labelChampionship.Name = "labelChampionship";
            labelChampionship.Size = new Size(110, 15);
            labelChampionship.TabIndex = 0;
            labelChampionship.Text = "labelChampionship";
            // 
            // comboBoxChampionship
            // 
            comboBoxChampionship.FormattingEnabled = true;
            comboBoxChampionship.Location = new Point(130, 3);
            comboBoxChampionship.Name = "comboBoxChampionship";
            comboBoxChampionship.Size = new Size(121, 23);
            comboBoxChampionship.TabIndex = 1;
            comboBoxChampionship.SelectionChangeCommitted += ComboBoxChampionship_SelectionChangeCommitted;
            //
            // tableLayoutPanel4
            //
            tableLayoutPanel4.AutoSize = true;
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(labelFavouriteTeam, 0, 0);
            tableLayoutPanel4.Controls.Add(comboBoxFavouriteTeam, 1, 0);
            tableLayoutPanel4.Location = new Point(0, 58);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(254, 29);
            tableLayoutPanel4.TabIndex = 3;
            //
            // labelFavouriteTeam
            //
            labelFavouriteTeam.Anchor = AnchorStyles.Left;
            labelFavouriteTeam.AutoSize = true;
            labelFavouriteTeam.Location = new Point(3, 7);
            labelFavouriteTeam.Name = "labelFavouriteTeam";
            labelFavouriteTeam.Size = new Size(108, 15);
            labelFavouriteTeam.TabIndex = 0;
            labelFavouriteTeam.Text = "labelFavouriteTeam";
            //
            // comboBoxFavouriteTeam
            //
            comboBoxFavouriteTeam.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFavouriteTeam.FormattingEnabled = true;
            comboBoxFavouriteTeam.Location = new Point(130, 3);
            comboBoxFavouriteTeam.Name = "comboBoxFavouriteTeam";
            comboBoxFavouriteTeam.Size = new Size(121, 23);
            comboBoxFavouriteTeam.TabIndex = 1;
            comboBoxFavouriteTeam.SelectionChangeCommitted += ComboBoxFavouriteTeam_SelectionChangeCommitted;
            //
            // buttonSave
            // 
            buttonSave.AutoSize = true;
            buttonSave.Dock = DockStyle.Bottom;
            buttonSave.Location = new Point(3, 61);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(248, 25);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "buttonSave";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            //
            // buttonTestApi
            //
            buttonTestApi.AutoSize = true;
            buttonTestApi.Dock = DockStyle.Bottom;
            buttonTestApi.Location = new Point(3, 92);
            buttonTestApi.Name = "buttonTestApi";
            buttonTestApi.Size = new Size(248, 25);
            buttonTestApi.TabIndex = 3;
            buttonTestApi.Text = "Test API Connection";
            buttonTestApi.UseVisualStyleBackColor = true;
            buttonTestApi.Click += ButtonTestApi_Click;
            //
            // buttonViewRankings
            //
            buttonViewRankings.AutoSize = true;
            buttonViewRankings.Dock = DockStyle.Bottom;
            buttonViewRankings.Location = new Point(3, 123);
            buttonViewRankings.Name = "buttonViewRankings";
            buttonViewRankings.Size = new Size(248, 25);
            buttonViewRankings.TabIndex = 4;
            buttonViewRankings.Text = "View Rankings";
            buttonViewRankings.UseVisualStyleBackColor = true;
            buttonViewRankings.Click += ButtonViewRankings_Click;
            //
            // buttonPlayerImages
            //
            buttonPlayerImages.AutoSize = true;
            buttonPlayerImages.Dock = DockStyle.Bottom;
            buttonPlayerImages.Location = new Point(3, 154);
            buttonPlayerImages.Name = "buttonPlayerImages";
            buttonPlayerImages.Size = new Size(248, 25);
            buttonPlayerImages.TabIndex = 5;
            buttonPlayerImages.Text = "Manage Player Images";
            buttonPlayerImages.UseVisualStyleBackColor = true;
            buttonPlayerImages.Click += ButtonPlayerImages_Click;
            //
            // buttonFavouritePlayers
            //
            buttonFavouritePlayers.AutoSize = true;
            buttonFavouritePlayers.Dock = DockStyle.Bottom;
            buttonFavouritePlayers.Location = new Point(3, 185);
            buttonFavouritePlayers.Name = "buttonFavouritePlayers";
            buttonFavouritePlayers.Size = new Size(248, 25);
            buttonFavouritePlayers.TabIndex = 6;
            buttonFavouritePlayers.Text = "Select Favourite Players";
            buttonFavouritePlayers.UseVisualStyleBackColor = true;
            buttonFavouritePlayers.Click += ButtonFavouritePlayers_Click;
            //
            // progressBarLoading
            //
            progressBarLoading = new ProgressBar();
            progressBarLoading.Dock = DockStyle.Bottom;
            progressBarLoading.Location = new Point(0, 421);
            progressBarLoading.Name = "progressBarLoading";
            progressBarLoading.Size = new Size(838, 20);
            progressBarLoading.Style = ProgressBarStyle.Marquee;
            progressBarLoading.MarqueeAnimationSpeed = 30;
            progressBarLoading.TabIndex = 7;
            progressBarLoading.Visible = false;
            //
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(838, 441);
            tableLayoutPanel3.TabIndex = 1;
            //
            // SettingsForm
            //
            AcceptButton = buttonSave;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(838, 441);
            Controls.Add(progressBarLoading);
            Controls.Add(tableLayoutPanel3);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(480, 320);
            Name = "SettingsForm";
            Text = "SettingsForm";
            FormClosing += SettingsForm_FormClosing;
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label labelChampionship;
        private ComboBox comboBoxChampionship;
        private TableLayoutPanel tableLayoutPanel2;
        private Label labelLanguage;
        private ComboBox comboBoxLanguage;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Label labelFavouriteTeam;
        private ComboBox comboBoxFavouriteTeam;
        private Button buttonSave;
        private Button buttonTestApi;
        private Button buttonViewRankings;
        private Button buttonPlayerImages;
        private Button buttonFavouritePlayers;
        private ProgressBar progressBarLoading;
    }
}
