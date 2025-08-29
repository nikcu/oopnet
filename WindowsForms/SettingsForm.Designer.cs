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
            buttonSave = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
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
            flowLayoutPanel1.Controls.Add(buttonSave);
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
            labelLanguage.Size = new Size(65, 15);
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
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(838, 441);
            Controls.Add(tableLayoutPanel3);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(480, 320);
            Name = "SettingsForm";
            Text = "SettingsForm";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
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
        private Button buttonSave;
    }
}
