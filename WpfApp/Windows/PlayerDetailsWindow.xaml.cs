using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Models;
using Utils;
using Utils.Helpers;

namespace WpfApp.Windows
{
    public partial class PlayerDetailsWindow : Window
    {
        // Dependencies (using interfaces for DIP)
        private readonly ISettingsService _settings;

        private readonly Player _player;
        private readonly string _teamCode;
        private readonly Match _match;

        public PlayerDetailsWindow(Player player, string teamCode, Match match)
        {
            InitializeComponent();

            // Initialize dependencies
            _settings = Settings.Instance;

            _player = player;
            _teamCode = teamCode;
            _match = match;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayPlayerInfo();
            LoadPlayerImage();
            CalculateMatchStats();
        }

        private void DisplayPlayerInfo()
        {
            textPlayerName.Text = _player.Name;
            textShirtNumber.Text = $"#{_player.ShirtNumber}";
            textPosition.Text = _player.Position;

            // Show captain badge if applicable
            if (_player.Captain)
            {
                textCaptain.Visibility = Visibility.Visible;
            }

            // Find team name
            string teamName = _teamCode;
            if (_match.HomeTeam.Code == _teamCode)
            {
                teamName = _match.HomeTeam.Country;
            }
            else if (_match.AwayTeam.Code == _teamCode)
            {
                teamName = _match.AwayTeam.Country;
            }
            textTeam.Text = teamName;
        }

        private void LoadPlayerImage()
        {
            try
            {
                string playerIdentifier = Constant.GeneratePlayerIdentifier(_player.Name, _player.ShirtNumber);
                string? imagePath = _settings.GetPlayerImagePath(playerIdentifier);

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    playerImage.ImageSource = bitmap;
                }
                else
                {
                    // Create placeholder with initials
                    CreatePlaceholderImage();
                }
            }
            catch
            {
                CreatePlaceholderImage();
            }
        }

        private void CreatePlaceholderImage()
        {
            // Get initials and color using shared helper
            string initials = PlayerImageHelper.GetInitials(_player.Name);
            var (r, g, b) = PlayerImageHelper.GetPlayerColorRgb(_player.Name);
            Color bgColor = Color.FromRgb((byte)r, (byte)g, (byte)b);

            // Create a DrawingVisual for the placeholder
            var drawingVisual = new DrawingVisual();
            using (var context = drawingVisual.RenderOpen())
            {
                // Draw background circle
                context.DrawEllipse(
                    new SolidColorBrush(bgColor),
                    null,
                    new Point(50, 50),
                    50, 50);

                // Draw initials
                var formattedText = new FormattedText(
                    initials,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    36,
                    Brushes.White,
                    96);

                formattedText.TextAlignment = TextAlignment.Center;
                context.DrawText(formattedText, new Point(50, 50 - formattedText.Height / 2));
            }

            // Render to bitmap
            var renderBitmap = new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            playerImage.ImageSource = renderBitmap;
        }

        private void CalculateMatchStats()
        {
            int goals = 0;
            int yellowCards = 0;

            // Get the events for the player's team
            List<MatchEvent> events;
            if (_match.HomeTeam.Code == _teamCode)
            {
                events = _match.HomeTeamEvents ?? new List<MatchEvent>();
            }
            else
            {
                events = _match.AwayTeamEvents ?? new List<MatchEvent>();
            }

            // Count goals and yellow cards for this player
            foreach (var evt in events)
            {
                if (evt.Player == _player.Name)
                {
                    if (evt.TypeOfEvent == "goal" || evt.TypeOfEvent == "goal-penalty")
                    {
                        goals++;
                    }
                    else if (evt.TypeOfEvent == "yellow-card")
                    {
                        yellowCards++;
                    }
                }
            }

            textGoals.Text = goals.ToString();
            textYellowCards.Text = yellowCards.ToString();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
