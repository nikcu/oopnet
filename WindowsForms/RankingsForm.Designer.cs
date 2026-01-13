namespace WindowsForms
{
    partial class RankingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RankingsForm));
            tabControl = new TabControl();
            tabPageGoalScorers = new TabPage();
            dataGridViewGoalScorers = new DataGridView();
            tabPageYellowCards = new TabPage();
            dataGridViewYellowCards = new DataGridView();
            tabPageAttendance = new TabPage();
            dataGridViewAttendance = new DataGridView();
            labelFavouriteTeam = new Label();
            buttonPrint = new Button();
            panelSelectedPlayer = new Panel();
            labelSelectedPlayer = new Label();
            tabControl.SuspendLayout();
            tabPageGoalScorers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewGoalScorers).BeginInit();
            tabPageYellowCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewYellowCards).BeginInit();
            tabPageAttendance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewAttendance).BeginInit();
            SuspendLayout();
            //
            // tabControl
            //
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabPageGoalScorers);
            tabControl.Controls.Add(tabPageYellowCards);
            tabControl.Controls.Add(tabPageAttendance);
            tabControl.Location = new Point(12, 50);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(760, 310);
            tabControl.TabIndex = 0;
            //
            // tabPageGoalScorers
            //
            tabPageGoalScorers.Controls.Add(dataGridViewGoalScorers);
            tabPageGoalScorers.Location = new Point(4, 24);
            tabPageGoalScorers.Name = "tabPageGoalScorers";
            tabPageGoalScorers.Padding = new Padding(3);
            tabPageGoalScorers.Size = new Size(752, 372);
            tabPageGoalScorers.TabIndex = 0;
            tabPageGoalScorers.Text = "Goal Scorers";
            tabPageGoalScorers.UseVisualStyleBackColor = true;
            //
            // dataGridViewGoalScorers
            //
            dataGridViewGoalScorers.AllowUserToAddRows = false;
            dataGridViewGoalScorers.AllowUserToDeleteRows = false;
            dataGridViewGoalScorers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewGoalScorers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewGoalScorers.Dock = DockStyle.Fill;
            dataGridViewGoalScorers.Location = new Point(3, 3);
            dataGridViewGoalScorers.Name = "dataGridViewGoalScorers";
            dataGridViewGoalScorers.ReadOnly = true;
            dataGridViewGoalScorers.RowHeadersVisible = false;
            dataGridViewGoalScorers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewGoalScorers.Size = new Size(746, 276);
            dataGridViewGoalScorers.TabIndex = 0;
            dataGridViewGoalScorers.SelectionChanged += DataGridViewGoalScorers_SelectionChanged;
            //
            // tabPageYellowCards
            //
            tabPageYellowCards.Controls.Add(dataGridViewYellowCards);
            tabPageYellowCards.Location = new Point(4, 24);
            tabPageYellowCards.Name = "tabPageYellowCards";
            tabPageYellowCards.Padding = new Padding(3);
            tabPageYellowCards.Size = new Size(752, 372);
            tabPageYellowCards.TabIndex = 1;
            tabPageYellowCards.Text = "Yellow Cards";
            tabPageYellowCards.UseVisualStyleBackColor = true;
            //
            // dataGridViewYellowCards
            //
            dataGridViewYellowCards.AllowUserToAddRows = false;
            dataGridViewYellowCards.AllowUserToDeleteRows = false;
            dataGridViewYellowCards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewYellowCards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewYellowCards.Dock = DockStyle.Fill;
            dataGridViewYellowCards.Location = new Point(3, 3);
            dataGridViewYellowCards.Name = "dataGridViewYellowCards";
            dataGridViewYellowCards.ReadOnly = true;
            dataGridViewYellowCards.RowHeadersVisible = false;
            dataGridViewYellowCards.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewYellowCards.Size = new Size(746, 276);
            dataGridViewYellowCards.TabIndex = 0;
            dataGridViewYellowCards.SelectionChanged += DataGridViewYellowCards_SelectionChanged;
            //
            // tabPageAttendance
            //
            tabPageAttendance.Controls.Add(dataGridViewAttendance);
            tabPageAttendance.Location = new Point(4, 24);
            tabPageAttendance.Name = "tabPageAttendance";
            tabPageAttendance.Size = new Size(752, 372);
            tabPageAttendance.TabIndex = 2;
            tabPageAttendance.Text = "Attendance";
            tabPageAttendance.UseVisualStyleBackColor = true;
            //
            // dataGridViewAttendance
            //
            dataGridViewAttendance.AllowUserToAddRows = false;
            dataGridViewAttendance.AllowUserToDeleteRows = false;
            dataGridViewAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewAttendance.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewAttendance.Dock = DockStyle.Fill;
            dataGridViewAttendance.Location = new Point(0, 0);
            dataGridViewAttendance.Name = "dataGridViewAttendance";
            dataGridViewAttendance.ReadOnly = true;
            dataGridViewAttendance.RowHeadersVisible = false;
            dataGridViewAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewAttendance.Size = new Size(752, 282);
            dataGridViewAttendance.TabIndex = 0;
            //
            // labelFavouriteTeam
            //
            labelFavouriteTeam.AutoSize = true;
            labelFavouriteTeam.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelFavouriteTeam.Location = new Point(16, 15);
            labelFavouriteTeam.Name = "labelFavouriteTeam";
            labelFavouriteTeam.Size = new Size(200, 21);
            labelFavouriteTeam.TabIndex = 1;
            labelFavouriteTeam.Text = "Rankings for: [Team Name]";
            //
            // buttonPrint
            //
            buttonPrint.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonPrint.Location = new Point(659, 12);
            buttonPrint.Name = "buttonPrint";
            buttonPrint.Size = new Size(113, 30);
            buttonPrint.TabIndex = 2;
            buttonPrint.Text = "Print / Export";
            buttonPrint.UseVisualStyleBackColor = true;
            buttonPrint.Click += buttonPrint_Click;
            //
            // panelSelectedPlayer
            //
            panelSelectedPlayer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelSelectedPlayer.BorderStyle = BorderStyle.FixedSingle;
            panelSelectedPlayer.Controls.Add(labelSelectedPlayer);
            panelSelectedPlayer.Location = new Point(12, 370);
            panelSelectedPlayer.Name = "panelSelectedPlayer";
            panelSelectedPlayer.Size = new Size(760, 80);
            panelSelectedPlayer.TabIndex = 3;
            //
            // labelSelectedPlayer
            //
            labelSelectedPlayer.Dock = DockStyle.Fill;
            labelSelectedPlayer.Font = new Font("Segoe UI", 9F);
            labelSelectedPlayer.ForeColor = Color.Gray;
            labelSelectedPlayer.Location = new Point(0, 0);
            labelSelectedPlayer.Name = "labelSelectedPlayer";
            labelSelectedPlayer.Size = new Size(758, 78);
            labelSelectedPlayer.TabIndex = 0;
            labelSelectedPlayer.Text = "Select a player from the list above";
            labelSelectedPlayer.TextAlign = ContentAlignment.MiddleCenter;
            //
            // RankingsForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 462);
            Controls.Add(panelSelectedPlayer);
            Controls.Add(buttonPrint);
            Controls.Add(labelFavouriteTeam);
            Controls.Add(tabControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 500);
            Name = "RankingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rankings";
            Load += RankingsForm_Load;
            tabControl.ResumeLayout(false);
            tabPageGoalScorers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewGoalScorers).EndInit();
            tabPageYellowCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewYellowCards).EndInit();
            tabPageAttendance.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewAttendance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl;
        private TabPage tabPageGoalScorers;
        private DataGridView dataGridViewGoalScorers;
        private TabPage tabPageYellowCards;
        private DataGridView dataGridViewYellowCards;
        private TabPage tabPageAttendance;
        private DataGridView dataGridViewAttendance;
        private Label labelFavouriteTeam;
        private Button buttonPrint;
        private Panel panelSelectedPlayer;
        private Label labelSelectedPlayer;
    }
}
