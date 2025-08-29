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
            Text = Translations.SettingsFormText;
            labelLanguage.Text = Translations.labelLanguageText;
            labelChampionship.Text = Translations.labelChampionshipText;
            ComboBoxLanguageInit();
        }

        private void ComboBoxLanguageInit()
        {
            TranslationOption[] translationOptions = [
                new TranslationOption { Label = Translations.stringCroatian, Value = "hr" },
                new TranslationOption { Label = Translations.stringEnglish, Value = "en" },
            ];


            comboBoxLanguage.Items.Clear();
            comboBoxLanguage.Items.AddRange(translationOptions);
            comboBoxLanguage.DisplayMember = TranslationOption.DisplayMember;
            comboBoxLanguage.ValueMember = TranslationOption.ValueMember;

            for (int i = 0; i < comboBoxLanguage.Items.Count; i++)
            {
                if (comboBoxLanguage.Items[i] is not TranslationOption option)
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

        private void ComboBoxLanguage_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox comboBoxLanguage = (ComboBox)sender;

            if (comboBoxLanguage.SelectedItem is not TranslationOption selectedOption)
            {
                return;
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(selectedOption.Value);
            SettingsFormAfterInit();
        }
    }
}