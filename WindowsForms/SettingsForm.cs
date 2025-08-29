namespace WindowsForms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        public void SettingsForm_Load(object sender, EventArgs e)
        {
            comboBoxLanguage.Items.AddRange(new object[] { "English", "Croatian" });
            comboBoxChampionship.Items.AddRange(new object[] { "Women", "Men" });
        }
    }
}
