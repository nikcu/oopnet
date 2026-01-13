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
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        private readonly string _championship;
        private readonly string? _favouriteTeamFifaCode;
        private List<Player> _players = new();
        private PlayerUserControl? _selectedPlayerControl;

        public PlayerImagesForm()
        {
            InitializeComponent();

            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;

            _championship = _settings.SelectedChampionship;
            _favouriteTeamFifaCode = _settings.FavouriteTeamFifaCode;
        }

        private async void PlayerImagesForm_Load(object sender, EventArgs e)
        {
            Text = Translations.StringPlayerImages;
            labelPlayerList.Text = Translations.StringPlayer;
            buttonBrowseImage.Text = Translations.StringBrowseImage;
            buttonRemoveImage.Text = Translations.StringRemoveImage;
            buttonFetchFromApi.Text = Translations.StringFetchFromApi;
            labelInstruction.Text = Translations.StringSelectPlayer;

            if (string.IsNullOrEmpty(_favouriteTeamFifaCode))
            {
                MessageBox.Show(
                    Translations.StringSelectFavouriteTeamFirst,
                    Translations.StringWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                Close();
                return;
            }

            await LoadPlayersAsync();
        }

        private async Task LoadPlayersAsync()
        {
            try
            {
                var matches = await _repository.GetCountryMatchesAsync(_championship, _favouriteTeamFifaCode!);

                if (matches == null || matches.Count == 0)
                {
                    MessageBox.Show(
                        Translations.StringNoMatchesFound,
                        Translations.StringWarning,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                var firstMatch = matches[0];
                var allPlayers = new List<Player>();

                bool isHomeTeam = firstMatch.HomeTeam.Code == _favouriteTeamFifaCode;
                var teamStats = isHomeTeam ? firstMatch.HomeTeamStatistics : firstMatch.AwayTeamStatistics;

                allPlayers.AddRange(teamStats.StartingEleven);
                allPlayers.AddRange(teamStats.Substitutes);

                _players = allPlayers.OrderBy(p => p.ShirtNumber).ToList();

                flowLayoutPlayers.SuspendLayout();
                flowLayoutPlayers.Controls.Clear();

                foreach (var player in _players)
                {
                    var playerControl = new PlayerUserControl();
                    playerControl.SetPlayer(player);
                    playerControl.Width = flowLayoutPlayers.Width - 25;
                    playerControl.Cursor = Cursors.Hand;
                    playerControl.PlayerClicked += PlayerControl_Clicked;
                    flowLayoutPlayers.Controls.Add(playerControl);
                }

                flowLayoutPlayers.ResumeLayout();

                if (flowLayoutPlayers.Controls.Count > 0)
                {
                    SelectPlayerControl(flowLayoutPlayers.Controls[0] as PlayerUserControl);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Translations.StringErrorLoadingData,
                    Translations.StringError,
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

            string captainIndicator = _selectedPlayerControl.IsCaptain ? " (C)" : "";
            labelPlayerInfo.Text = $"#{_selectedPlayerControl.ShirtNumber} {_selectedPlayerControl.PlayerName}{captainIndicator} - {_selectedPlayerControl.Position}";

            buttonBrowseImage.Enabled = true;
            buttonFetchFromApi.Enabled = true;

            string identifier = _selectedPlayerControl.PlayerIdentifier;
            string? imagePath = _settings.GetPlayerImagePath(identifier);

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        pictureBoxPlayer.Image = Image.FromStream(stream);
                    }

                    labelInstruction.Visible = false;
                    pictureBoxPlayer.Visible = true;
                    buttonRemoveImage.Enabled = true;
                }
                catch
                {
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
                    buttonBrowseImage.Enabled = false;
                    string originalButtonText = buttonBrowseImage.Text;
                    buttonBrowseImage.Text = Translations.StringLoading;

                    try
                    {
                        string sourceFile = openFileDialog.FileName;
                        string extension = Path.GetExtension(sourceFile);

                        string targetPath = Constant.GetPlayerImagePath(
                            _championship,
                            _favouriteTeamFifaCode!,
                            _selectedPlayerControl.PlayerName,
                            _selectedPlayerControl.ShirtNumber,
                            extension
                        );

                        string? targetDirectory = Path.GetDirectoryName(targetPath);
                        if (!string.IsNullOrEmpty(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        await Task.Run(() => File.Copy(sourceFile, targetPath, overwrite: true));

                        _settings.SetPlayerImagePath(_selectedPlayerControl.PlayerIdentifier, targetPath);
                        await _settings.SaveSettingsAsync();

                        _selectedPlayerControl.LoadPlayerImage();
                        UpdateSelectedPlayerDisplay();

                        MessageBox.Show(
                            Translations.StringImageAssigned,
                            Translations.StringSuccess,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(
                            Translations.StringErrorLoadingImage,
                            Translations.StringError,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                    finally
                    {
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
                Translations.StringRemoveConfirmation,
                Translations.StringConfirm,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                buttonRemoveImage.Enabled = false;

                try
                {
                    string? imagePath = _settings.GetPlayerImagePath(_selectedPlayerControl.PlayerIdentifier);

                    _settings.RemovePlayerImagePath(_selectedPlayerControl.PlayerIdentifier);
                    await _settings.SaveSettingsAsync();

                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        try
                        {
                            await Task.Run(() => File.Delete(imagePath));
                        }
                        catch { }
                    }

                    _selectedPlayerControl.LoadPlayerImage();
                    UpdateSelectedPlayerDisplay();
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        Translations.StringErrorLoadingImage,
                        Translations.StringError,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                finally
                {
                    buttonRemoveImage.Enabled = true;
                }
            }
        }

        private async void ButtonFetchFromApi_Click(object sender, EventArgs e)
        {
            if (_selectedPlayerControl == null)
                return;

            buttonFetchFromApi.Enabled = false;
            string originalButtonText = buttonFetchFromApi.Text;
            buttonFetchFromApi.Text = Translations.StringLoading;

            try
            {
                string playerSearchName = _selectedPlayerControl.PlayerName.Replace(" ", "_");
                string apiUrl = $"https://www.thesportsdb.com/api/v1/json/3/searchplayers.php?p={playerSearchName}";

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await httpClient.GetStringAsync(apiUrl);

                using var jsonDoc = System.Text.Json.JsonDocument.Parse(response);
                var root = jsonDoc.RootElement;

                if (!root.TryGetProperty("player", out var playersArray) ||
                    playersArray.ValueKind == System.Text.Json.JsonValueKind.Null ||
                    playersArray.GetArrayLength() == 0)
                {
                    MessageBox.Show(
                        Translations.StringPlayerNotFound,
                        Translations.StringWarning,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                var playerData = playersArray[0];

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
                        Translations.StringNoImageAvailable,
                        Translations.StringWarning,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                string extension = Path.GetExtension(imageUrl);
                if (string.IsNullOrEmpty(extension) || extension.Length > 5)
                {
                    extension = ".jpg";
                }

                string targetPath = Constant.GetPlayerImagePath(
                    _championship,
                    _favouriteTeamFifaCode!,
                    _selectedPlayerControl.PlayerName,
                    _selectedPlayerControl.ShirtNumber,
                    extension
                );

                string? targetDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                await Task.Run(() => File.WriteAllBytes(targetPath, imageBytes));

                _settings.SetPlayerImagePath(_selectedPlayerControl.PlayerIdentifier, targetPath);
                await _settings.SaveSettingsAsync();

                _selectedPlayerControl.LoadPlayerImage();
                UpdateSelectedPlayerDisplay();

                MessageBox.Show(
                    Translations.StringImageAssigned,
                    Translations.StringSuccess,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(
                    Translations.StringNetworkError,
                    Translations.StringError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show(
                    Translations.StringTimeoutError,
                    Translations.StringWarning,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Translations.StringErrorLoadingImage,
                    Translations.StringError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                buttonFetchFromApi.Text = originalButtonText;
                buttonFetchFromApi.Enabled = true;
            }
        }
    }
}
