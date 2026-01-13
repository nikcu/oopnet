using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using Utils;
using WpfApp.Windows;
using WpfComboBoxItem = System.Windows.Controls.ComboBoxItem;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        // Dependencies (using interfaces for DIP)
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        private List<TeamResult> _teams = new();
        private List<Match> _allMatches = new();
        private Match? _selectedMatch;
        private string _championship = "m";

        public MainWindow()
        {
            InitializeComponent();

            // Initialize dependencies
            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply saved settings
            ApplySettings();
            ApplyLocalization();

            // Load data
            await LoadTeamsAsync();
        }

        private void ApplySettings()
        {
            // Apply culture
            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(_settings.SelectedLanguage);
            }
            catch
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            }

            _championship = _settings.SelectedChampionship;

            // Apply display mode
            if (_settings.WpfIsFullscreen)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
                Width = _settings.WpfResolutionWidth;
                Height = _settings.WpfResolutionHeight;
            }
        }

        private void ApplyLocalization()
        {
            Title = $"World Cup 2018 - {(_championship == "m" ? Translations.StringMens : Translations.StringWomens)}";
            labelFavouriteTeam.Content = Translations.StringFavouriteTeam;
            labelOpponent.Content = Translations.StringOpponent;
            buttonSettings.Content = Translations.StringSettings;
            textLoading.Text = Translations.StringLoading;
        }

        #region Data Loading

        private async Task LoadTeamsAsync()
        {
            try
            {
                ShowLoading(true);
                textStatus.Text = Translations.StringLoading;

                var teams = await _repository.GetTeamResultsAsync(_championship);

                if (teams != null && teams.Count > 0)
                {
                    _teams = teams.OrderBy(t => t.Country).ToList();

                    // Also load all matches for opponent filtering
                    _allMatches = await _repository.GetAllMatchesAsync(_championship) ?? new List<Match>();

                    PopulateFavouriteTeamComboBox();
                    textStatus.Text = "";
                }
                else
                {
                    textStatus.Text = Translations.StringNoTeamsAvailable;
                    MessageBox.Show(
                        Translations.StringNoTeamsAvailable,
                        Translations.StringWarning,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading teams: {ex.Message}");
                textStatus.Text = Translations.StringError;
                MessageBox.Show(
                    Translations.StringErrorLoadingTeams,
                    Translations.StringError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void PopulateFavouriteTeamComboBox()
        {
            comboBoxFavouriteTeam.Items.Clear();

            foreach (var team in _teams)
            {
                var item = new WpfComboBoxItem
                {
                    Content = $"{team.Country} ({team.FifaCode})",
                    Tag = team.FifaCode
                };
                comboBoxFavouriteTeam.Items.Add(item);

                // Select saved favourite
                if (team.FifaCode == _settings.FavouriteTeamFifaCode)
                {
                    comboBoxFavouriteTeam.SelectedItem = item;
                }
            }

            // Default to first if no saved favourite
            if (comboBoxFavouriteTeam.SelectedItem == null && comboBoxFavouriteTeam.Items.Count > 0)
            {
                comboBoxFavouriteTeam.SelectedIndex = 0;
            }
        }

        private void PopulateOpponentComboBox(string favouriteTeamCode)
        {
            comboBoxOpponent.Items.Clear();

            // Find all teams that played against the favourite team
            var opponentCodes = new HashSet<string>();

            foreach (var match in _allMatches)
            {
                if (match.HomeTeam.Code == favouriteTeamCode)
                {
                    opponentCodes.Add(match.AwayTeam.Code);
                }
                else if (match.AwayTeam.Code == favouriteTeamCode)
                {
                    opponentCodes.Add(match.HomeTeam.Code);
                }
            }

            // Add opponent teams to combo box
            var opponents = _teams.Where(t => opponentCodes.Contains(t.FifaCode)).OrderBy(t => t.Country);

            foreach (var team in opponents)
            {
                var item = new WpfComboBoxItem
                {
                    Content = $"{team.Country} ({team.FifaCode})",
                    Tag = team.FifaCode
                };
                comboBoxOpponent.Items.Add(item);
            }

            if (comboBoxOpponent.Items.Count > 0)
            {
                comboBoxOpponent.SelectedIndex = 0;
            }
        }

        #endregion

        #region Event Handlers

        private async void ComboBoxFavouriteTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxFavouriteTeam.SelectedItem is WpfComboBoxItem item && item.Tag != null)
            {
                string teamCode = item.Tag.ToString()!;
                _settings.FavouriteTeamFifaCode = teamCode;
                PopulateOpponentComboBox(teamCode);

                // Save to settings file
                await _settings.SaveSettingsAsync();
            }
        }

        private void ComboBoxOpponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxFavouriteTeam.SelectedItem is WpfComboBoxItem favItem &&
                comboBoxOpponent.SelectedItem is WpfComboBoxItem oppItem &&
                favItem.Tag != null && oppItem.Tag != null)
            {
                string favCode = favItem.Tag.ToString()!;
                string oppCode = oppItem.Tag.ToString()!;

                // Find the match between these two teams
                _selectedMatch = _allMatches.FirstOrDefault(m =>
                    (m.HomeTeam.Code == favCode && m.AwayTeam.Code == oppCode) ||
                    (m.HomeTeam.Code == oppCode && m.AwayTeam.Code == favCode));

                if (_selectedMatch != null)
                {
                    DisplayMatch(_selectedMatch);
                }
            }
        }

        private void DisplayMatch(Match match)
        {
            textNoMatch.Visibility = Visibility.Collapsed;
            borderMatchResult.Visibility = Visibility.Visible;
            borderField.Visibility = Visibility.Visible;

            // Display team names
            textHomeTeam.Text = $"{match.HomeTeam.Country} ({match.HomeTeam.Code})";
            textAwayTeam.Text = $"{match.AwayTeam.Country} ({match.AwayTeam.Code})";

            // Display score
            textScore.Text = $"{match.HomeTeam.Goals} : {match.AwayTeam.Goals}";
            textMatchInfo.Text = $"{match.StageName} - {match.Location}";

            // Store team codes for info buttons
            buttonHomeTeamInfo.Tag = match.HomeTeam.Code;
            buttonAwayTeamInfo.Tag = match.AwayTeam.Code;

            textStatus.Text = $"Showing: {match.HomeTeam.Country} vs {match.AwayTeam.Country}";

            // Display lineups on field
            DisplayLineups(match);
        }

        private void DisplayLineups(Match match)
        {
            // Clear existing lineups
            ClearAllLineups();

            // Update lineup titles
            textHomeLineupTitle.Text = match.HomeTeam.Country;
            textAwayLineupTitle.Text = match.AwayTeam.Country;

            int playerIndex = 0;

            // Display home team starting eleven by position (animate from right/center)
            if (match.HomeTeamStatistics?.StartingEleven != null)
            {
                foreach (var player in match.HomeTeamStatistics.StartingEleven)
                {
                    var playerControl = CreatePlayerControl(player, match.HomeTeam.Code);
                    var targetControl = GetHomePositionControl(player.Position);
                    targetControl.Items.Add(playerControl);

                    // Animate from center (right side for home team)
                    AnimatePlayerEntry(playerControl, 150, playerIndex * 80);
                    playerIndex++;
                }
            }

            // Display away team starting eleven by position (animate from left/center)
            if (match.AwayTeamStatistics?.StartingEleven != null)
            {
                foreach (var player in match.AwayTeamStatistics.StartingEleven)
                {
                    var playerControl = CreatePlayerControl(player, match.AwayTeam.Code);
                    var targetControl = GetAwayPositionControl(player.Position);
                    targetControl.Items.Add(playerControl);

                    // Animate from center (left side for away team)
                    AnimatePlayerEntry(playerControl, -150, playerIndex * 80);
                    playerIndex++;
                }
            }
        }

        private void AnimatePlayerEntry(Border playerControl, double fromX, int delayMs)
        {
            // Set up transform for animation
            var transform = new TranslateTransform(fromX, 0);
            playerControl.RenderTransform = transform;
            playerControl.Opacity = 0;

            // Create slide animation
            var slideAnimation = new DoubleAnimation
            {
                From = fromX,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(400),
                BeginTime = TimeSpan.FromMilliseconds(delayMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Create fade animation
            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                BeginTime = TimeSpan.FromMilliseconds(delayMs)
            };

            // Apply animations
            transform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
            playerControl.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private void ClearAllLineups()
        {
            itemsHomeGoalies.Items.Clear();
            itemsHomeDefenders.Items.Clear();
            itemsHomeMidfielders.Items.Clear();
            itemsHomeForwards.Items.Clear();
            itemsAwayGoalies.Items.Clear();
            itemsAwayDefenders.Items.Clear();
            itemsAwayMidfielders.Items.Clear();
            itemsAwayForwards.Items.Clear();
        }

        private ItemsControl GetHomePositionControl(string position)
        {
            return position?.ToLower() switch
            {
                "goalie" => itemsHomeGoalies,
                "defender" => itemsHomeDefenders,
                "midfield" => itemsHomeMidfielders,
                "forward" => itemsHomeForwards,
                _ => itemsHomeMidfielders // Default to midfield
            };
        }

        private ItemsControl GetAwayPositionControl(string position)
        {
            return position?.ToLower() switch
            {
                "goalie" => itemsAwayGoalies,
                "defender" => itemsAwayDefenders,
                "midfield" => itemsAwayMidfielders,
                "forward" => itemsAwayForwards,
                _ => itemsAwayMidfielders // Default to midfield
            };
        }

        private Border CreatePlayerControl(Player player, string teamCode)
        {
            var border = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(200, 255, 255, 255)),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(2),
                Cursor = Cursors.Hand,
                Tag = new PlayerInfo { Player = player, TeamCode = teamCode },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            // Shirt number
            stackPanel.Children.Add(new TextBlock
            {
                Text = $"#{player.ShirtNumber}",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 5, 0),
                FontSize = 11
            });

            // Player name
            stackPanel.Children.Add(new TextBlock
            {
                Text = player.Name + (player.Captain ? " (C)" : ""),
                FontSize = 11
            });

            border.Child = stackPanel;

            // Click handler for player details
            border.MouseLeftButtonUp += PlayerControl_Click;

            return border;
        }

        private void PlayerControl_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is PlayerInfo playerInfo && _selectedMatch != null)
            {
                var playerDetailsWindow = new PlayerDetailsWindow(
                    playerInfo.Player, playerInfo.TeamCode, _selectedMatch);
                playerDetailsWindow.Owner = this;
                playerDetailsWindow.ShowDialog();
            }
        }

        private void ButtonHomeTeamInfo_Click(object sender, RoutedEventArgs e)
        {
            if (buttonHomeTeamInfo.Tag is string teamCode)
            {
                ShowTeamInfo(teamCode);
            }
        }

        private void ButtonAwayTeamInfo_Click(object sender, RoutedEventArgs e)
        {
            if (buttonAwayTeamInfo.Tag is string teamCode)
            {
                ShowTeamInfo(teamCode);
            }
        }

        private void ShowTeamInfo(string teamCode)
        {
            var team = _teams.FirstOrDefault(t => t.FifaCode == teamCode);
            if (team != null)
            {
                var teamInfoWindow = new TeamInfoWindow(team);
                teamInfoWindow.Owner = this;
                teamInfoWindow.ShowDialog();
            }
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;

            if (settingsWindow.ShowDialog() == true)
            {
                // Reload with new settings
                ApplySettings();
                ApplyLocalization();
                _ = LoadTeamsAsync();
            }
        }

        #endregion

        #region Helpers

        private void ShowLoading(bool show)
        {
            loadingPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            // Only show "no match" message if not loading AND no match is selected
            if (show)
            {
                textNoMatch.Visibility = Visibility.Collapsed;
            }
            else
            {
                textNoMatch.Visibility = _selectedMatch == null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region Window Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                Translations.StringCloseConfirmation,
                "World Cup 2018",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        #endregion

        // Helper class to store player info with team code
        private class PlayerInfo
        {
            public Player Player { get; set; } = null!;
            public string TeamCode { get; set; } = "";
        }
    }
}
