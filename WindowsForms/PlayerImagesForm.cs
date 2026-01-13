using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using Utils;
using WindowsForms.UserControls;

namespace WindowsForms
{
    public partial class PlayerImagesForm : Form
    {
        // Dependencies (using interfaces for DIP)
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        private readonly string _championship;
        private readonly string? _favouriteTeamFifaCode;
        private List<Player> _players = new();
        private PlayerUserControl? _selectedPlayerControl;

        public PlayerImagesForm()
        {
            InitializeComponent();

            // Initialize dependencies
            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;

            _championship = _settings.SelectedChampionship;
            _favouriteTeamFifaCode = _settings.FavouriteTeamFifaCode;
        }

        private async void PlayerImagesForm_Load(object sender, EventArgs e)
        {
            // Set localized text
            Text = Translations.StringPlayerImages;
            labelPlayerList.Text = Translations.StringPlayer;
            buttonBrowseImage.Text = Translations.StringBrowseImage;
            buttonRemoveImage.Text = Translations.StringRemoveImage;
            buttonFetchFromApi.Text = Translations.StringFetchFromApi;
            labelInstruction.Text = Translations.StringSelectPlayer;

            // Check if favourite team is selected
            if (string.IsNullOrEmpty(_favouriteTeamFifaCode))
            {
                MessageBox.Show(
                    "Please select a favourite team first in the Settings.",
                    "No Favourite Team",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                Close();
                return;
            }

            // Load players
            await LoadPlayersAsync();
        }

        private async Task LoadPlayersAsync()
        {
            try
            {
                Console.WriteLine($"Loading players for team: {_favouriteTeamFifaCode}");

                var matches = await _repository.GetCountryMatchesAsync(_championship, _favouriteTeamFifaCode!);

                if (matches == null || matches.Count == 0)
                {
                    MessageBox.Show(
                        "No matches found for the selected team.",
                        "No Data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Get players from first match (starting_eleven + substitutes)
                var firstMatch = matches[0];
                var allPlayers = new List<Player>();

                // Determine if favourite team is home or away
                bool isHomeTeam = firstMatch.HomeTeam.Code == _favouriteTeamFifaCode;
                var teamStats = isHomeTeam ? firstMatch.HomeTeamStatistics : firstMatch.AwayTeamStatistics;

                allPlayers.AddRange(teamStats.StartingEleven);
                allPlayers.AddRange(teamStats.Substitutes);

                // Sort by shirt number
                _players = allPlayers.OrderBy(p => p.ShirtNumber).ToList();

                Console.WriteLine($"Loaded {_players.Count} players");

                // Populate FlowLayoutPanel with PlayerUserControl instances
                flowLayoutPlayers.SuspendLayout();
                flowLayoutPlayers.Controls.Clear();

                foreach (var player in _players)
                {
                    var playerControl = new PlayerUserControl();
                    playerControl.SetPlayer(player);
                    playerControl.Width = flowLayoutPlayers.Width - 25; // Account for scrollbar
                    playerControl.Cursor = Cursors.Hand;
                    playerControl.PlayerClicked += PlayerControl_Clicked;
                    flowLayoutPlayers.Controls.Add(playerControl);
                }

                flowLayoutPlayers.ResumeLayout();

                // Select first player
                if (flowLayoutPlayers.Controls.Count > 0)
                {
                    SelectPlayerControl(flowLayoutPlayers.Controls[0] as PlayerUserControl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading players: {ex.Message}");
                MessageBox.Show(
                    $"Failed to load players.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void PlayerControl_Clicked(object? sender, EventArgs e)
        {
            if (sender is PlayerUserControl control)
            {
                SelectPlayerControl(control);
            }
        }

        private void SelectPlayerControl(PlayerUserControl? control)
        {
            // Deselect previous
            if (_selectedPlayerControl != null)
            {
                _selectedPlayerControl.IsSelected = false;
            }

            _selectedPlayerControl = control;

            if (control != null)
            {
                control.IsSelected = true;
                UpdateSelectedPlayerDisplay();
            }
            else
            {
                ClearPlayerDisplay();
            }
        }

        private void UpdateSelectedPlayerDisplay()
        {
            if (_selectedPlayerControl == null)
            {
                ClearPlayerDisplay();
                return;
            }

            // Update player info label
            string captainIndicator = _selectedPlayerControl.IsCaptain ? " (C)" : "";
            labelPlayerInfo.Text = $"#{_selectedPlayerControl.ShirtNumber} {_selectedPlayerControl.PlayerName}{captainIndicator} - {_selectedPlayerControl.Position}";

            // Enable buttons
            buttonBrowseImage.Enabled = true;
            buttonFetchFromApi.Enabled = true;

            // Load player image if exists
            string identifier = _selectedPlayerControl.PlayerIdentifier;
            string? imagePath = _settings.GetPlayerImagePath(identifier);

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    // Load image from file
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pictureBoxPlayer.Image = Image.FromStream(stream);
                    }

                    labelInstruction.Visible = false;
                    pictureBoxPlayer.Visible = true;
                    buttonRemoveImage.Enabled = true;

                    Console.WriteLine($"Loaded image for {_selectedPlayerControl.PlayerName}: {imagePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                    ShowNoImage();
                }
            }
            else
            {
                ShowNoImage();
            }
        }

        private void ClearPlayerDisplay()
        {
            pictureBoxPlayer.Image = null;
            labelInstruction.Visible = true;
            pictureBoxPlayer.Visible = false;
            buttonBrowseImage.Enabled = false;
            buttonRemoveImage.Enabled = false;
            buttonFetchFromApi.Enabled = false;
            labelPlayerInfo.Text = Translations.StringSelectPlayer;
        }

        private void ShowNoImage()
        {
            pictureBoxPlayer.Image = null;
            pictureBoxPlayer.Visible = false;
            labelInstruction.Visible = true;
            labelInstruction.Text = Translations.StringNoImageAssigned;
            buttonRemoveImage.Enabled = false;
        }

        private async void ButtonBrowseImage_Click(object sender, EventArgs e)
        {
            if (_selectedPlayerControl == null)
                return;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = Translations.StringBrowseImage;
                openFileDialog.Filter = $"{Translations.StringImageFiles}|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Disable button to prevent double-clicks
                    buttonBrowseImage.Enabled = false;
                    string originalButtonText = buttonBrowseImage.Text;
                    buttonBrowseImage.Text = Translations.StringLoading;

                    try
                    {
                        string sourceFile = openFileDialog.FileName;
                        string extension = Path.GetExtension(sourceFile);

                        // Generate target path
                        string targetPath = Constant.GetPlayerImagePath(
                            _championship,
                            _favouriteTeamFifaCode!,
                            _selectedPlayerControl.PlayerName,
                            _selectedPlayerControl.ShirtNumber,
                            extension
                        );

                        // Ensure target directory exists
                        string? targetDirectory = Path.GetDirectoryName(targetPath);
                        if (!string.IsNullOrEmpty(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                            Console.WriteLine($"Created directory: {targetDirectory}");
                        }

                        // Copy file asynchronously to target location
                        await Task.Run(() => File.Copy(sourceFile, targetPath, overwrite: true));
                        Console.WriteLine($"Copied image to: {targetPath}");

                        // Save mapping in Settings
                        _settings.SetPlayerImagePath(_selectedPlayerControl.PlayerIdentifier, targetPath);
                        await _settings.SaveSettingsAsync();

                        Console.WriteLine($"Saved image mapping for {_selectedPlayerControl.PlayerIdentifier}");

                        // Refresh the PlayerUserControl's image
                        _selectedPlayerControl.LoadPlayerImage();

                        // Refresh the large preview display
                        UpdateSelectedPlayerDisplay();

                        MessageBox.Show(
                            $"Image assigned to {_selectedPlayerControl.PlayerName} successfully!",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error assigning image: {ex.Message}");
                        MessageBox.Show(
                            $"Failed to assign image.\n\n{ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    finally
                    {
                        // Re-enable button
                        buttonBrowseImage.Text = originalButtonText;
                        buttonBrowseImage.Enabled = true;
                    }
                }
            }
        }

        private async void ButtonRemoveImage_Click(object sender, EventArgs e)
        {
            if (_selectedPlayerControl == null)
                return;

            var result = MessageBox.Show(
                $"Remove image for {_selectedPlayerControl.PlayerName}?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Disable button during operation
                buttonRemoveImage.Enabled = false;

                try
                {
                    string? imagePath = _settings.GetPlayerImagePath(_selectedPlayerControl.PlayerIdentifier);

                    // Remove from settings
                    _settings.RemovePlayerImagePath(_selectedPlayerControl.PlayerIdentifier);
                    await _settings.SaveSettingsAsync();

                    // Optionally delete the file asynchronously
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        try
                        {
                            await Task.Run(() => File.Delete(imagePath));
                            Console.WriteLine($"Deleted image file: {imagePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not delete image file: {ex.Message}");
                        }
                    }

                    // Refresh the PlayerUserControl's image (will show placeholder)
                    _selectedPlayerControl.LoadPlayerImage();

                    // Refresh the large preview display
                    UpdateSelectedPlayerDisplay();

                    Console.WriteLine($"Removed image for {_selectedPlayerControl.PlayerIdentifier}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error removing image: {ex.Message}");
                    MessageBox.Show(
                        $"Failed to remove image.\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                finally
                {
                    // Re-enable button (will be updated by refresh)
                    buttonRemoveImage.Enabled = true;
                }
            }
        }

        private async void ButtonFetchFromApi_Click(object sender, EventArgs e)
        {
            if (_selectedPlayerControl == null)
                return;

            // Disable button during operation
            buttonFetchFromApi.Enabled = false;
            string originalButtonText = buttonFetchFromApi.Text;
            buttonFetchFromApi.Text = Translations.StringLoading;

            try
            {
                // Search for player on TheSportsDB API
                string playerSearchName = _selectedPlayerControl.PlayerName.Replace(" ", "_");
                string apiUrl = $"https://www.thesportsdb.com/api/v1/json/3/searchplayers.php?p={playerSearchName}";

                Console.WriteLine($"Searching TheSportsDB API: {apiUrl}");

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await httpClient.GetStringAsync(apiUrl);

                // Parse JSON response
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(response);
                var root = jsonDoc.RootElement;

                // Check if player found
                if (!root.TryGetProperty("player", out var playersArray) ||
                    playersArray.ValueKind == System.Text.Json.JsonValueKind.Null ||
                    playersArray.GetArrayLength() == 0)
                {
                    MessageBox.Show(
                        $"Player '{_selectedPlayerControl.PlayerName}' not found in TheSportsDB.\n\n" +
                        $"Try searching manually or use the Browse Image button.",
                        "Player Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Get first player result
                var playerData = playersArray[0];

                // Try to get image URLs in order of preference: cutout, render, thumb
                string? imageUrl = null;
                if (playerData.TryGetProperty("strCutout", out var cutout) &&
                    cutout.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    imageUrl = cutout.GetString();
                }

                if (string.IsNullOrEmpty(imageUrl) &&
                    playerData.TryGetProperty("strRender", out var render) &&
                    render.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    imageUrl = render.GetString();
                }

                if (string.IsNullOrEmpty(imageUrl) &&
                    playerData.TryGetProperty("strThumb", out var thumb) &&
                    thumb.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    imageUrl = thumb.GetString();
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    MessageBox.Show(
                        $"No image found for '{_selectedPlayerControl.PlayerName}' in TheSportsDB.\n\n" +
                        $"The API returned player data but no image URLs.",
                        "No Image Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                Console.WriteLine($"Found player image URL: {imageUrl}");

                // Download image
                byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                // Determine file extension from URL or default to .jpg
                string extension = Path.GetExtension(imageUrl);
                if (string.IsNullOrEmpty(extension) || extension.Length > 5)
                {
                    extension = ".jpg";
                }

                // Generate target path
                string targetPath = Constant.GetPlayerImagePath(
                    _championship,
                    _favouriteTeamFifaCode!,
                    _selectedPlayerControl.PlayerName,
                    _selectedPlayerControl.ShirtNumber,
                    extension
                );

                // Ensure target directory exists
                string? targetDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                    Console.WriteLine($"Created directory: {targetDirectory}");
                }

                // Save image file asynchronously
                await Task.Run(() => File.WriteAllBytes(targetPath, imageBytes));
                Console.WriteLine($"Saved image to: {targetPath}");

                // Save mapping in Settings
                _settings.SetPlayerImagePath(_selectedPlayerControl.PlayerIdentifier, targetPath);
                await _settings.SaveSettingsAsync();

                Console.WriteLine($"Saved image mapping for {_selectedPlayerControl.PlayerIdentifier}");

                // Refresh the PlayerUserControl's image
                _selectedPlayerControl.LoadPlayerImage();

                // Refresh the large preview display
                UpdateSelectedPlayerDisplay();

                MessageBox.Show(
                    $"Image fetched from TheSportsDB and assigned to {_selectedPlayerControl.PlayerName} successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error fetching image: {ex.Message}");
                MessageBox.Show(
                    $"Failed to connect to TheSportsDB API.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Please check your internet connection or try again later.",
                    "Network Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out");
                MessageBox.Show(
                    "Request timed out. The API server may be slow or unreachable.\n\n" +
                    "Please try again later.",
                    "Timeout Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching image from API: {ex.Message}");
                MessageBox.Show(
                    $"Failed to fetch image from API.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Re-enable button
                buttonFetchFromApi.Text = originalButtonText;
                buttonFetchFromApi.Enabled = true;
            }
        }
    }
}
