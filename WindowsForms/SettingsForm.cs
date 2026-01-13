using System.CodeDom.Compiler;
using System.Globalization;
using Utils;
using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Repositories;

namespace WindowsForms
{
    public partial class SettingsForm : Form
    {
        // Dependencies (using interfaces for DIP)
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        public SettingsForm()
        {
            InitializeComponent();

            // Initialize dependencies
            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;

            SettingsFormAfterInit();
        }

        private void SettingsFormAfterInit()
        {
            // Set culture from saved settings on first load
            if (_settings.GetIsLoadedFromFile())
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(_settings.SelectedLanguage);
                }
                catch (CultureNotFoundException)
                {
                    // If invalid culture, default to English
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
            _ = ComboBoxFavouriteTeamInitAsync(); // Fire and forget - loads teams asynchronously
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

            // Select current language from settings (which should match current culture)
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

            // Default to first item if no match found
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

            // Update culture for UI
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(selectedItem.Value);

            // Save language to settings
            _settings.SelectedLanguage = selectedItem.Value;

            // Refresh UI with new language
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

            // Reload favourite team combo when championship changes
            _ = ComboBoxFavouriteTeamInitAsync();
        }

        private async Task ComboBoxFavouriteTeamInitAsync()
        {
            ComboBox thisBox = comboBoxFavouriteTeam;

            // Disable combo while loading and show progress bar
            thisBox.Enabled = false;
            progressBarLoading.Visible = true;
            thisBox.Items.Clear();
            thisBox.Items.Add(new ComboBoxItem { Label = Translations.StringLoading, Value = "" });
            thisBox.DisplayMember = ComboBoxItem.DisplayMember;
            thisBox.ValueMember = ComboBoxItem.ValueMember;
            thisBox.SelectedIndex = 0;

            try
            {
                // Get current championship
                string championship = _settings.SelectedChampionship;

                // Load teams from API via DataLayer
                var teams = await _repository.GetTeamResultsAsync(championship);

                if (teams == null || teams.Count == 0)
                {
                    Console.WriteLine("No teams found from API");
                    thisBox.Items.Clear();
                    thisBox.Items.Add(new ComboBoxItem { Label = "No teams available", Value = "" });
                    thisBox.SelectedIndex = 0;
                    return;
                }

                Console.WriteLine($"Loaded {teams.Count} teams from API");

                // Clear loading message and populate with teams
                thisBox.Items.Clear();

                // Sort teams by country name for better UX
                var sortedTeams = teams.OrderBy(t => t.Country).ToList();

                foreach (var team in sortedTeams)
                {
                    // Format: "COUNTRY (FIFA_CODE)"
                    string label = $"{team.Country} ({team.FifaCode})";
                    thisBox.Items.Add(new ComboBoxItem { Label = label, Value = team.FifaCode });
                }

                // Try to select the saved favourite team
                string? savedFavouriteTeam = _settings.FavouriteTeamFifaCode;
                if (!string.IsNullOrEmpty(savedFavouriteTeam))
                {
                    for (int i = 0; i < thisBox.Items.Count; i++)
                    {
                        if (thisBox.Items[i] is ComboBoxItem item && item.Value == savedFavouriteTeam)
                        {
                            thisBox.SelectedIndex = i;
                            Console.WriteLine($"Selected saved favourite team: {savedFavouriteTeam}");
                            break;
                        }
                    }
                }

                // If no saved favourite or not found, default to first item
                if (thisBox.SelectedIndex == -1 && thisBox.Items.Count > 0)
                {
                    thisBox.SelectedIndex = 0;

                    // Automatically save the first team as favourite if no favourite is set
                    if (thisBox.Items[0] is ComboBoxItem firstItem)
                    {
                        _settings.FavouriteTeamFifaCode = firstItem.Value;
                        Console.WriteLine($"Auto-selected first team as favourite: {firstItem.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading teams: {ex.Message}");
                thisBox.Items.Clear();
                thisBox.Items.Add(new ComboBoxItem { Label = "Error loading teams", Value = "" });
                thisBox.SelectedIndex = 0;

                MessageBox.Show(
                    $"Failed to load teams from API.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Please check your internet connection or API configuration.",
                    "Load Teams Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            finally
            {
                // Re-enable combo after loading and hide progress bar
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

            // Save selected team to settings
            _settings.FavouriteTeamFifaCode = selectedItem.Value;
            Console.WriteLine($"Favourite team changed to: {selectedItem.Value}");
        }

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            // Disable button during save operation and show loading indicator
            buttonSave.Enabled = false;
            progressBarLoading.Visible = true;
            string originalText = buttonSave.Text;
            buttonSave.Text = Translations.StringLoading;

            try
            {
                // Get current selected language from ComboBox
                if (comboBoxLanguage.SelectedItem is ComboBoxItem languageItem)
                {
                    _settings.SelectedLanguage = languageItem.Value;
                }

                // Get current selected championship from ComboBox
                if (comboBoxChampionship.SelectedItem is ComboBoxItem championshipItem)
                {
                    _settings.SelectedChampionship = championshipItem.Value;
                }

                // Save settings asynchronously
                await _settings.SaveSettingsAsync();

                // Show success message
                string favouriteTeamDisplay = _settings.FavouriteTeamFifaCode ?? "None";
                MessageBox.Show(
                    $"Settings saved successfully!\n\n" +
                    $"Championship: {(_settings.SelectedChampionship == "m" ? "Men's" : "Women's")}\n" +
                    $"Language: {(_settings.SelectedLanguage == "en" ? "English" : "Croatian")}\n" +
                    $"Favourite Team: {favouriteTeamDisplay}\n" +
                    $"File: {Path.GetFullPath(DataLayer.Constant.pathSettings)}",
                    "Settings Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    $"Permission Error: Unable to save settings file.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Try running the application as administrator or check file permissions.",
                    "Permission Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show(
                    $"Directory Error: The settings directory could not be found.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"The directory will be created automatically on next save attempt.",
                    "Directory Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    $"File Error: Unable to write settings file.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"The file may be in use by another application or the disk may be full.",
                    "File Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unexpected Error: Failed to save settings.\n\n" +
                    $"Error Type: {ex.GetType().Name}\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Stack Trace:\n{ex.StackTrace}",
                    "Save Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Re-enable button and hide loading indicator
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

                // Test fetching team results
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
            // Check if favourite team is selected
            if (string.IsNullOrEmpty(_settings.FavouriteTeamFifaCode))
            {
                var result = MessageBox.Show(
                    "No favourite team selected. Rankings will show all teams.\n\n" +
                    "Do you want to continue?",
                    "No Favourite Team",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                    return;
            }

            // Open rankings form
            using (var rankingsForm = new RankingsForm())
            {
                rankingsForm.ShowDialog(this);
            }
        }

        private void ButtonPlayerImages_Click(object sender, EventArgs e)
        {
            // Open player images form
            using (var playerImagesForm = new PlayerImagesForm())
            {
                playerImagesForm.ShowDialog(this);
            }
        }

        private void ButtonFavouritePlayers_Click(object sender, EventArgs e)
        {
            // Check if favourite team is selected
            if (string.IsNullOrEmpty(_settings.FavouriteTeamFifaCode))
            {
                MessageBox.Show(
                    "Please select a favourite team first before selecting favourite players.",
                    "No Favourite Team",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // Open favourite players form
            using (var favouritePlayersForm = new FavoritePlayersForm())
            {
                favouritePlayersForm.ShowDialog(this);
            }
        }

        #region Close Confirmation

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Show confirmation dialog before closing
            // Enter = confirm close (Yes), Esc = cancel close (No)
            var result = MessageBox.Show(
                Translations.StringCloseConfirmation,
                Translations.StringSettings,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1 // Default to "Yes" so Enter confirms close
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
                    // Esc key closes the form (will trigger FormClosing confirmation)
                    Close();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        #endregion
    }
}