using System.CodeDom.Compiler;
using System.Globalization;
using Utils;
using DataLayer;

namespace WindowsForms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            SettingsFormAfterInit();
        }

        private void SettingsFormAfterInit()
        {
            Text = Translations.StringSettings;
            labelLanguage.Text = Translations.StringLanguage;
            labelChampionship.Text = Translations.StringChampionship;
            buttonSave.Text = Translations.StringSave;
            ComboBoxLanguageInit();
            ComboBoxChampionshipInit();
        }

        private void ComboBoxLanguageInit()
        {
            ComboBox thisBox = comboBoxLanguage;

            thisBox.Items.Clear();
            thisBox.Items.AddRange([
                new ComboBoxItem { Label = Translations.StringLangCroatian, Value = "hr" },
                new ComboBoxItem { Label = Translations.StringLangEnglish, Value = "en" },
            ]);
            thisBox.DisplayMember = ComboBoxItem.DisplayMember;
            thisBox.ValueMember = ComboBoxItem.ValueMember;

            for (int i = 0; i < thisBox.Items.Count; i++)
            {
                if (thisBox.Items[i] is not ComboBoxItem option)
                {
                    continue;
                }

                if (option.Value == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName)
                {
                    thisBox.SelectedIndex = i;
                    return;
                }
            }

            thisBox.SelectedIndex = 0;
        }

        private void ComboBoxChampionshipInit()
        {
            ComboBox thisBox = comboBoxChampionship;

            thisBox.Items.Clear();
            thisBox.Items.AddRange([
                new ComboBoxItem { Label = Translations.StringWomens, Value = "f" },
                new ComboBoxItem { Label = Translations.StringMens, Value = "m" },
            ]);
            thisBox.DisplayMember = ComboBoxItem.DisplayMember;
            thisBox.ValueMember = ComboBoxItem.ValueMember;

            for (int i = 0; i < thisBox.Items.Count; i++)
            {
                if (thisBox.Items[i] is not ComboBoxItem option)
                {
                    continue;
                }

                if (option.Value == Settings.Instance.SelectedChampionship)
                {
                    thisBox.SelectedIndex = i;
                    return;
                }
            }

            thisBox.SelectedIndex = 0;
        }

        private void ComboBoxLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox thisBox = (ComboBox)sender;

            if (thisBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(selectedItem.Value);
            SettingsFormAfterInit();
        }

        private void comboBoxChampionship_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox thisBox = (ComboBox)sender;

            if (thisBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            Settings.Instance.SelectedChampionship = selectedItem.Value;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Settings.Instance.SaveSettings();
        }
    }
}