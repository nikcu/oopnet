using System.CodeDom.Compiler;
using System.Globalization;
using Utils;

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
            comboBoxLanguage.Items.Clear();
            comboBoxLanguage.Items.AddRange([
                new ComboBoxItem { Label = Translations.StringLangCroatian, Value = "hr" },
                new ComboBoxItem { Label = Translations.StringLangEnglish, Value = "en" },
            ]);
            comboBoxLanguage.DisplayMember = ComboBoxItem.DisplayMember;
            comboBoxLanguage.ValueMember = ComboBoxItem.ValueMember;

            for (int i = 0; i < comboBoxLanguage.Items.Count; i++)
            {
                if (comboBoxLanguage.Items[i] is not ComboBoxItem option)
                {
                    continue;
                }

                if (option.Value == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName)
                {
                    comboBoxLanguage.SelectedIndex = i;
                    return;
                }
            }

            comboBoxLanguage.SelectedIndex = 0;
        }

        private void ComboBoxChampionshipInit()
        {
            comboBoxChampionship.Items.Clear();
            comboBoxChampionship.Items.AddRange([
                new ComboBoxItem { Label = Translations.StringWomens, Value = "F" },
                new ComboBoxItem { Label = Translations.StringMens, Value = "M" },
            ]);
            comboBoxChampionship.DisplayMember = ComboBoxItem.DisplayMember;
            comboBoxChampionship.ValueMember = ComboBoxItem.ValueMember;

            comboBoxChampionship.SelectedIndex = 0;
        }

        private void ComboBoxLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBoxLanguage = (ComboBox)sender;

            if (comboBoxLanguage.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(selectedItem.Value);
            SettingsFormAfterInit();
        }
    }
}