using System.Globalization;
using Utils;
using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Repositories;

namespace WindowsForms
{
    public partial class SettingsForm : Form
    {
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        public SettingsForm()
        {
            InitializeComponent();

            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;

            SettingsFormAfterInit();
        }

        private void SettingsFormAfterInit()
        {
            if (_settings.GetIsLoadedFromFile())
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(_settings.SelectedLanguage);
                }
                catch (CultureNotFoundException)
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    _settings.SelectedLanguage = "en";
                }
            }

            Text = Translations.StringSettings;
            labelLanguage.Text = Translations.StringLanguage;
            labelChampionship.Text = Translations.StringChampionship;
            labelFavouriteTeam.Text = Translations.StringFavouriteTeam;
            buttonSave.Text = Translations.StringSave;
            buttonViewRankings.Text = Translations.StringViewRankings;
            buttonPlayerImages.Text = Translations.StringManagePlayerImages;
            ComboBoxLanguageInit();
            ComboBoxChampionshipInit();
            _ = ComboBoxFavouriteTeamInitAsync();
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

            string currentLanguage = _settings.SelectedLanguage;

            for (int i = 0; i < thisBox.Items.Count; i++)
            {
                if (thisBox.Items[i] is not ComboBoxItem option)
                {
                    continue;
                }

                if (option.Value == currentLanguage)
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

                if (option.Value == _settings.SelectedChampionship)
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
            _settings.SelectedLanguage = selectedItem.Value;
            SettingsFormAfterInit();
        }

        private void ComboBoxChampionship_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox thisBox = (ComboBox)sender;

            if (thisBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            _settings.SelectedChampionship = selectedItem.Value;
            _ = ComboBoxFavouriteTeamInitAsync();
        }

        private async Task ComboBoxFavouriteTeamInitAsync()
        {
            ComboBox thisBox = comboBoxFavouriteTeam;

            thisBox.Enabled = false;
            progressBarLoading.Visible = true;
            thisBox.Items.Clear();
            thisBox.Items.Add(new ComboBoxItem { Label = Translations.StringLoading, Value = "" });
            thisBox.DisplayMember = ComboBoxItem.DisplayMember;
            thisBox.ValueMember = ComboBoxItem.ValueMember;
            thisBox.SelectedIndex = 0;

            try
            {
                string championship = _settings.SelectedChampionship;
                var teams = await _repository.GetTeamResultsAsync(championship);

                if (teams == null || teams.Count == 0)
                {
                    thisBox.Items.Clear();
                    thisBox.Items.Add(new ComboBoxItem { Label = Translations.StringNoTeamsAvailable, Value = "" });
                    thisBox.SelectedIndex = 0;
                    return;
                }

                thisBox.Items.Clear();

                var sortedTeams = teams.OrderBy(t => t.Country).ToList();

                foreach (var team in sortedTeams)
                {
                    string label = $"{team.Country} ({team.FifaCode})";
                    thisBox.Items.Add(new ComboBoxItem { Label = label, Value = team.FifaCode });
                }

                string? savedFavouriteTeam = _settings.FavouriteTeamFifaCode;
                if (!string.IsNullOrEmpty(savedFavouriteTeam))
                {
                    for (int i = 0; i < thisBox.Items.Count; i++)
                    {
                        if (thisBox.Items[i] is ComboBoxItem item && item.Value == savedFavouriteTeam)
                        {
                            thisBox.SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (thisBox.SelectedIndex == -1 && thisBox.Items.Count > 0)
                {
                    thisBox.SelectedIndex = 0;

                    if (thisBox.Items[0] is ComboBoxItem firstItem)
                    {
                        _settings.FavouriteTeamFifaCode = firstItem.Value;
                    }
                }
            }
            catch (Exception)
            {
                thisBox.Items.Clear();
                thisBox.Items.Add(new ComboBoxItem { Label = Translations.StringError, Value = "" });
                thisBox.SelectedIndex = 0;

                MessageBox.Show(
                    Translations.StringErrorLoadingTeams,
                    Translations.StringError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            finally
            {
                thisBox.Enabled = true;
                progressBarLoading.Visible = false;
            }
        }

        private void ComboBoxFavouriteTeam_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox thisBox = (ComboBox)sender;

            if (thisBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            _settings.FavouriteTeamFifaCode = selectedItem.Value;
        }

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            buttonSave.Enabled = false;
            progressBarLoading.Visible = true;
            string originalText = buttonSave.Text;
            buttonSave.Text = Translations.StringLoading;

            try
            {
                if (comboBoxLanguage.SelectedItem is ComboBoxItem languageItem)
                {
                    _settings.SelectedLanguage = languageItem.Value;
                }

                if (comboBoxChampionship.SelectedItem is ComboBoxItem championshipItem)
                {
                    _settings.SelectedChampionship = championshipItem.Value;
                }

                await _settings.SaveSettingsAsync();

                MessageBox.Show(
                    Translations.StringSettingsSaved,
                    Translations.StringSuccess,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Translations.StringErrorSavingSettings,
                    Translations.StringError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                buttonSave.Enabled = true;
                buttonSave.Text = originalText;
                progressBarLoading.Visible = false;
            }
        }

        private async void ButtonTestApi_Click(object sender, EventArgs e)
        {
            buttonTestApi.Enabled = false;
            buttonTestApi.Text = Translations.StringLoading;
            progressBarLoading.Visible = true;

            try
            {
                var championship = _settings.SelectedChampionship;
                var teams = await _repository.GetTeamResultsAsync(championship);

                if (teams != null && teams.Count > 0)
                {
                    MessageBox.Show(
                        $"API Connection Successful!\n\n" +
                        $"Data Source: {_repository.GetDataSource()}\n" +
                        $"Championship: {(championship == "m" ? "Men's" : "Women's")}\n" +
                        $"Teams Found: {teams.Count}\n\n" +
                        $"Sample Team: {teams[0].Country} ({teams[0].FifaCode})",
                        "API Test Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "API connection succeeded but no data was returned.",
                        "API Test Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"API Test Failed!\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Details: {ex.InnerException?.Message}",
                    "API Test Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                buttonTestApi.Enabled = true;
                buttonTestApi.Text = "Test API Connection";
                progressBarLoading.Visible = false;
            }
        }

        private void ButtonViewRankings_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_settings.FavouriteTeamFifaCode))
            {
                var result = MessageBox.Show(
                    $"{Translations.StringNoFavouriteTeam} {Translations.StringContinueQuestion}",
                    Translations.StringWarning,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                    return;
            }

            using (var rankingsForm = new RankingsForm())
            {
                rankingsForm.ShowDialog(this);
            }
        }

        private void ButtonPlayerImages_Click(object sender, EventArgs e)
        {
            using (var playerImagesForm = new PlayerImagesForm())
            {
                playerImagesForm.ShowDialog(this);
            }
        }

        private void ButtonFavouritePlayers_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_settings.FavouriteTeamFifaCode))
            {
                MessageBox.Show(
                    Translations.StringSelectFavouriteTeamFirst,
                    Translations.StringWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            using (var favouritePlayersForm = new FavoritePlayersForm())
            {
                favouritePlayersForm.ShowDialog(this);
            }
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                Translations.StringCloseConfirmation,
                Translations.StringSettings,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    Close();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}
