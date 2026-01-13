using System.Drawing.Printing;
using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using DataLayer.Services;
using Utils;
using Utils.Helpers;
using WindowsForms.UserControls;

namespace WindowsForms
{
    public partial class RankingsForm : Form
    {
        // Dependencies (using interfaces for DIP)
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;
        private readonly RankingsService _rankingsService;

        private readonly string _championship;
        private readonly string? _favouriteTeamFifaCode;
        private int _currentPrintPage;
        private int _currentPrintRow;
        private Dictionary<string, Player> _playerLookup = new();
        private PlayerUserControl? _selectedPlayerControl;

        public RankingsForm()
        {
            InitializeComponent();

            // Initialize dependencies
            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;
            _rankingsService = new RankingsService();

            _championship = _settings.SelectedChampionship;
            _favouriteTeamFifaCode = _settings.FavouriteTeamFifaCode;
        }

        private async void RankingsForm_Load(object sender, EventArgs e)
        {
            // Set form title and labels
            Text = Translations.StringRankings;
            tabPageGoalScorers.Text = Translations.StringGoalScorers;
            tabPageYellowCards.Text = Translations.StringYellowCards;
            tabPageAttendance.Text = Translations.StringAttendance;
            buttonPrint.Text = Translations.StringPrintExport;

            // Update favourite team label
            if (!string.IsNullOrEmpty(_favouriteTeamFifaCode))
            {
                labelFavouriteTeam.Text = $"{Translations.StringRankings} for: {_favouriteTeamFifaCode}";
            }
            else
            {
                labelFavouriteTeam.Text = $"{Translations.StringRankings} (No favourite team selected)";
            }

            // Load data
            await LoadRankingsAsync();
        }

        private async Task LoadRankingsAsync()
        {
            try
            {
                // Show loading message in all grids
                Console.WriteLine($"Loading rankings for championship: {_championship}, favourite team: {_favouriteTeamFifaCode ?? "All"}");

                // Load matches using repository interface
                List<Match> matches;

                if (!string.IsNullOrEmpty(_favouriteTeamFifaCode))
                {
                    // Load matches for specific team
                    matches = await _repository.GetCountryMatchesAsync(_championship, _favouriteTeamFifaCode);
                    Console.WriteLine($"Loaded {matches?.Count ?? 0} matches for team {_favouriteTeamFifaCode}");
                }
                else
                {
                    // Load all matches
                    matches = await _repository.GetAllMatchesAsync(_championship);
                    Console.WriteLine($"Loaded {matches?.Count ?? 0} total matches");
                }

                if (matches == null || matches.Count == 0)
                {
                    MessageBox.Show(
                        "No match data available for the selected championship and team.",
                        "No Data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Build player lookup from match statistics
                BuildPlayerLookup(matches);

                // Process rankings
                LoadGoalScorersRanking(matches);
                LoadYellowCardsRanking(matches);
                LoadAttendanceRanking(matches);

                Console.WriteLine("Rankings loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading rankings: {ex.Message}");
                MessageBox.Show(
                    $"Failed to load rankings.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Please check your internet connection or API configuration.",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadGoalScorersRanking(List<Match> matches)
        {
            // Aggregate goals by player
            var goalScorers = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                // Process home team events
                foreach (var evt in match.HomeTeamEvents)
                {
                    if (evt.TypeOfEvent.Equals("goal", StringComparison.OrdinalIgnoreCase) ||
                        evt.TypeOfEvent.Equals("goal-penalty", StringComparison.OrdinalIgnoreCase))
                    {
                        string playerName = evt.Player;
                        if (!string.IsNullOrEmpty(playerName))
                        {
                            if (goalScorers.ContainsKey(playerName))
                                goalScorers[playerName]++;
                            else
                                goalScorers[playerName] = 1;
                        }
                    }
                }

                // Process away team events
                foreach (var evt in match.AwayTeamEvents)
                {
                    if (evt.TypeOfEvent.Equals("goal", StringComparison.OrdinalIgnoreCase) ||
                        evt.TypeOfEvent.Equals("goal-penalty", StringComparison.OrdinalIgnoreCase))
                    {
                        string playerName = evt.Player;
                        if (!string.IsNullOrEmpty(playerName))
                        {
                            if (goalScorers.ContainsKey(playerName))
                                goalScorers[playerName]++;
                            else
                                goalScorers[playerName] = 1;
                        }
                    }
                }
            }

            // Sort by goal count (descending)
            var sortedScorers = goalScorers
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .ToList();

            Console.WriteLine($"Found {sortedScorers.Count} goal scorers");

            // Clear and set up columns manually for image support
            dataGridViewGoalScorers.DataSource = null;
            dataGridViewGoalScorers.Columns.Clear();
            dataGridViewGoalScorers.Rows.Clear();

            // Add columns
            dataGridViewGoalScorers.Columns.Add("Rank", "#");
            dataGridViewGoalScorers.Columns["Rank"].Width = 40;

            var imageCol = new DataGridViewImageColumn
            {
                Name = "Image",
                HeaderText = "",
                Width = 40,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dataGridViewGoalScorers.Columns.Add(imageCol);

            dataGridViewGoalScorers.Columns.Add("Player", Translations.StringPlayer);
            dataGridViewGoalScorers.Columns["Player"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridViewGoalScorers.Columns.Add("Goals", Translations.StringGoals);
            dataGridViewGoalScorers.Columns["Goals"].Width = 60;

            // Set row height to accommodate images
            dataGridViewGoalScorers.RowTemplate.Height = 36;

            // Add rows with images
            int rank = 1;
            foreach (var scorer in sortedScorers)
            {
                var image = GetPlayerImage(scorer.Key);
                dataGridViewGoalScorers.Rows.Add(rank, image, scorer.Key, scorer.Value);
                rank++;
            }
        }

        private void LoadYellowCardsRanking(List<Match> matches)
        {
            // Aggregate yellow cards by player
            var yellowCards = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                // Process home team events
                foreach (var evt in match.HomeTeamEvents)
                {
                    if (evt.TypeOfEvent.Equals("yellow-card", StringComparison.OrdinalIgnoreCase))
                    {
                        string playerName = evt.Player;
                        if (!string.IsNullOrEmpty(playerName))
                        {
                            if (yellowCards.ContainsKey(playerName))
                                yellowCards[playerName]++;
                            else
                                yellowCards[playerName] = 1;
                        }
                    }
                }

                // Process away team events
                foreach (var evt in match.AwayTeamEvents)
                {
                    if (evt.TypeOfEvent.Equals("yellow-card", StringComparison.OrdinalIgnoreCase))
                    {
                        string playerName = evt.Player;
                        if (!string.IsNullOrEmpty(playerName))
                        {
                            if (yellowCards.ContainsKey(playerName))
                                yellowCards[playerName]++;
                            else
                                yellowCards[playerName] = 1;
                        }
                    }
                }
            }

            // Sort by card count (descending)
            var sortedCards = yellowCards
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .ToList();

            Console.WriteLine($"Found {sortedCards.Count} players with yellow cards");

            // Clear and set up columns manually for image support
            dataGridViewYellowCards.DataSource = null;
            dataGridViewYellowCards.Columns.Clear();
            dataGridViewYellowCards.Rows.Clear();

            // Add columns
            dataGridViewYellowCards.Columns.Add("Rank", "#");
            dataGridViewYellowCards.Columns["Rank"].Width = 40;

            var imageCol = new DataGridViewImageColumn
            {
                Name = "Image",
                HeaderText = "",
                Width = 40,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            dataGridViewYellowCards.Columns.Add(imageCol);

            dataGridViewYellowCards.Columns.Add("Player", Translations.StringPlayer);
            dataGridViewYellowCards.Columns["Player"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridViewYellowCards.Columns.Add("Cards", Translations.StringCards);
            dataGridViewYellowCards.Columns["Cards"].Width = 60;

            // Set row height to accommodate images
            dataGridViewYellowCards.RowTemplate.Height = 36;

            // Add rows with images
            int rank = 1;
            foreach (var card in sortedCards)
            {
                var image = GetPlayerImage(card.Key);
                dataGridViewYellowCards.Rows.Add(rank, image, card.Key, card.Value);
                rank++;
            }
        }

        private void LoadAttendanceRanking(List<Match> matches)
        {
            // Process attendance data
            var attendanceList = matches
                .Where(m => !string.IsNullOrEmpty(m.Attendance))
                .Select(m => new
                {
                    Location = m.Location,
                    Venue = m.Venue,
                    Attendance = ParseAttendance(m.Attendance),
                    HomeTeam = m.HomeTeamCountry,
                    AwayTeam = m.AwayTeamCountry,
                    Match = $"{m.HomeTeamCountry} vs {m.AwayTeamCountry}"
                })
                .Where(a => a.Attendance > 0)
                .OrderByDescending(a => a.Attendance)
                .Select((a, index) => new
                {
                    Rank = index + 1,
                    a.Location,
                    a.Attendance,
                    a.Match
                })
                .ToList();

            Console.WriteLine($"Found {attendanceList.Count} matches with attendance data");

            // Bind to grid
            dataGridViewAttendance.DataSource = attendanceList;

            // Configure columns (only if we have data)
            if (attendanceList.Count > 0 && dataGridViewAttendance.Columns.Count > 0)
            {
                try
                {
                    if (dataGridViewAttendance.Columns["Rank"] != null)
                    {
                        dataGridViewAttendance.Columns["Rank"].HeaderText = "#";
                        dataGridViewAttendance.Columns["Rank"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    if (dataGridViewAttendance.Columns["Location"] != null)
                    {
                        dataGridViewAttendance.Columns["Location"].HeaderText = Translations.StringLocation;
                    }
                    if (dataGridViewAttendance.Columns["Attendance"] != null)
                    {
                        dataGridViewAttendance.Columns["Attendance"].HeaderText = Translations.StringAttendance;
                        dataGridViewAttendance.Columns["Attendance"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dataGridViewAttendance.Columns["Attendance"].DefaultCellStyle.Format = "N0";
                    }
                    if (dataGridViewAttendance.Columns["Match"] != null)
                    {
                        dataGridViewAttendance.Columns["Match"].HeaderText = Translations.StringMatch;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring attendance columns: {ex.Message}");
                }
            }
        }

        private int ParseAttendance(string attendanceString)
        {
            // Try to parse attendance string to int
            // Handle cases like "45,000" or "45000"
            if (string.IsNullOrWhiteSpace(attendanceString))
                return 0;

            // Remove common separators
            string cleaned = attendanceString.Replace(",", "").Replace(".", "").Trim();

            if (int.TryParse(cleaned, out int result))
                return result;

            return 0;
        }

        private void BuildPlayerLookup(List<Match> matches)
        {
            _playerLookup.Clear();

            foreach (var match in matches)
            {
                // Add players from home team
                if (match.HomeTeamStatistics?.StartingEleven != null)
                {
                    foreach (var player in match.HomeTeamStatistics.StartingEleven)
                    {
                        if (!string.IsNullOrEmpty(player.Name) && !_playerLookup.ContainsKey(player.Name))
                        {
                            _playerLookup[player.Name] = player;
                        }
                    }
                }

                if (match.HomeTeamStatistics?.Substitutes != null)
                {
                    foreach (var player in match.HomeTeamStatistics.Substitutes)
                    {
                        if (!string.IsNullOrEmpty(player.Name) && !_playerLookup.ContainsKey(player.Name))
                        {
                            _playerLookup[player.Name] = player;
                        }
                    }
                }

                // Add players from away team
                if (match.AwayTeamStatistics?.StartingEleven != null)
                {
                    foreach (var player in match.AwayTeamStatistics.StartingEleven)
                    {
                        if (!string.IsNullOrEmpty(player.Name) && !_playerLookup.ContainsKey(player.Name))
                        {
                            _playerLookup[player.Name] = player;
                        }
                    }
                }

                if (match.AwayTeamStatistics?.Substitutes != null)
                {
                    foreach (var player in match.AwayTeamStatistics.Substitutes)
                    {
                        if (!string.IsNullOrEmpty(player.Name) && !_playerLookup.ContainsKey(player.Name))
                        {
                            _playerLookup[player.Name] = player;
                        }
                    }
                }
            }

            Console.WriteLine($"Built player lookup with {_playerLookup.Count} players");
        }

        private Image? GetPlayerImage(string playerName)
        {
            if (!_playerLookup.TryGetValue(playerName, out var player))
                return CreatePlaceholderImage(playerName);

            string identifier = Constant.GeneratePlayerIdentifier(player.Name, player.ShirtNumber);
            string? imagePath = Settings.Instance.GetPlayerImagePath(identifier);

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    return Image.FromStream(fs);
                }
                catch
                {
                    return CreatePlaceholderImage(playerName);
                }
            }

            return CreatePlaceholderImage(playerName);
        }

        private Image CreatePlaceholderImage(string playerName)
        {
            // Create a small circular placeholder with initials
            int size = 32;
            var bitmap = new Bitmap(size, size);

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Get color based on name using shared helper
            var (r, gb, b) = PlayerImageHelper.GetPlayerColorRgb(playerName);
            Color bgColor = Color.FromArgb(r, gb, b);
            using var brush = new SolidBrush(bgColor);
            g.FillEllipse(brush, 0, 0, size - 1, size - 1);

            // Draw initials using shared helper
            string initials = PlayerImageHelper.GetInitials(playerName);
            using var font = new Font("Segoe UI", 10, FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);

            var textSize = g.MeasureString(initials, font);
            float x = (size - textSize.Width) / 2;
            float y = (size - textSize.Height) / 2;
            g.DrawString(initials, font, textBrush, x, y);

            return bitmap;
        }

        #region Printing

        private void buttonPrint_Click(object? sender, EventArgs e)
        {
            var currentGrid = GetCurrentDataGridView();
            if (currentGrid == null || currentGrid.Rows.Count == 0)
            {
                MessageBox.Show(
                    Translations.StringNoDataToPrint,
                    Translations.StringPrint,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            using var printDocument = new PrintDocument();
            printDocument.DocumentName = GetPrintTitle();
            printDocument.PrintPage += PrintDocument_PrintPage;

            using var printPreviewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 800,
                Height = 600
            };

            _currentPrintPage = 0;
            _currentPrintRow = 0;

            printPreviewDialog.ShowDialog();
        }

        private DataGridView? GetCurrentDataGridView()
        {
            return tabControl.SelectedIndex switch
            {
                0 => dataGridViewGoalScorers,
                1 => dataGridViewYellowCards,
                2 => dataGridViewAttendance,
                _ => null
            };
        }

        private string GetPrintTitle()
        {
            string tabName = tabControl.SelectedIndex switch
            {
                0 => Translations.StringGoalScorers,
                1 => Translations.StringYellowCards,
                2 => Translations.StringAttendance,
                _ => "Rankings"
            };

            string teamInfo = !string.IsNullOrEmpty(_favouriteTeamFifaCode)
                ? $" - {_favouriteTeamFifaCode}"
                : "";

            return $"{tabName}{teamInfo}";
        }

        private void PrintDocument_PrintPage(object? sender, PrintPageEventArgs e)
        {
            if (e.Graphics == null) return;

            var grid = GetCurrentDataGridView();
            if (grid == null) return;

            // Page setup
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float pageWidth = e.MarginBounds.Width;
            float pageHeight = e.MarginBounds.Height;

            // Fonts
            using var titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
            using var headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
            using var cellFont = new Font("Segoe UI", 10);

            float yPos = topMargin;
            float rowHeight = 25f;

            // Print title on first page
            if (_currentPrintPage == 0 && _currentPrintRow == 0)
            {
                string title = GetPrintTitle();
                e.Graphics.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                yPos += titleFont.GetHeight(e.Graphics) + 20;

                // Print date
                string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                e.Graphics.DrawString(dateStr, cellFont, Brushes.Gray, leftMargin, yPos);
                yPos += cellFont.GetHeight(e.Graphics) + 20;
            }

            // Calculate column widths
            int visibleColumns = grid.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            float colWidth = pageWidth / visibleColumns;

            // Print headers on each page
            float xPos = leftMargin;
            using var headerBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
            e.Graphics.FillRectangle(headerBrush, leftMargin, yPos, pageWidth, rowHeight);

            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (!col.Visible) continue;
                e.Graphics.DrawString(col.HeaderText, headerFont, Brushes.Black,
                    new RectangleF(xPos, yPos + 3, colWidth, rowHeight));
                xPos += colWidth;
            }
            yPos += rowHeight;

            // Draw header line
            using var pen = new Pen(Color.Black, 1);
            e.Graphics.DrawLine(pen, leftMargin, yPos, leftMargin + pageWidth, yPos);
            yPos += 5;

            // Print rows
            int rowsPerPage = (int)((pageHeight - yPos + topMargin) / rowHeight);

            while (_currentPrintRow < grid.Rows.Count && yPos + rowHeight < topMargin + pageHeight)
            {
                var row = grid.Rows[_currentPrintRow];
                xPos = leftMargin;

                // Alternate row colors
                if (_currentPrintRow % 2 == 1)
                {
                    using var altBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
                    e.Graphics.FillRectangle(altBrush, leftMargin, yPos, pageWidth, rowHeight);
                }

                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (!col.Visible) continue;

                    var cellValue = row.Cells[col.Index].Value;

                    // Handle image columns
                    if (col is DataGridViewImageColumn && cellValue is Image img)
                    {
                        // Draw the image scaled to fit the row height
                        float imgSize = rowHeight - 4;
                        float imgX = xPos + 2;
                        float imgY = yPos + 2;
                        e.Graphics.DrawImage(img, imgX, imgY, imgSize, imgSize);
                    }
                    else
                    {
                        string textValue = cellValue?.ToString() ?? "";

                        // Format numbers with thousand separator for attendance
                        if (col.Name == "Attendance" && int.TryParse(textValue.Replace(",", ""), out int attendance))
                        {
                            textValue = attendance.ToString("N0");
                        }

                        e.Graphics.DrawString(textValue, cellFont, Brushes.Black,
                            new RectangleF(xPos, yPos + 3, colWidth, rowHeight));
                    }

                    xPos += colWidth;
                }

                yPos += rowHeight;
                _currentPrintRow++;
            }

            // Check if more pages needed
            e.HasMorePages = _currentPrintRow < grid.Rows.Count;
            if (e.HasMorePages)
            {
                _currentPrintPage++;
            }
        }

        #endregion

        #region Player Selection

        private void DataGridViewGoalScorers_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridViewGoalScorers.SelectedRows.Count == 0) return;

            var row = dataGridViewGoalScorers.SelectedRows[0];
            string? playerName = row.Cells["Player"]?.Value?.ToString();

            if (!string.IsNullOrEmpty(playerName))
            {
                ShowSelectedPlayer(playerName);
            }
        }

        private void DataGridViewYellowCards_SelectionChanged(object? sender, EventArgs e)
        {
            if (dataGridViewYellowCards.SelectedRows.Count == 0) return;

            var row = dataGridViewYellowCards.SelectedRows[0];
            string? playerName = row.Cells["Player"]?.Value?.ToString();

            if (!string.IsNullOrEmpty(playerName))
            {
                ShowSelectedPlayer(playerName);
            }
        }

        private void ShowSelectedPlayer(string playerName)
        {
            // Try to find player in lookup
            if (!_playerLookup.TryGetValue(playerName, out var player))
            {
                // Player not in lookup, show basic info
                labelSelectedPlayer.Visible = true;
                labelSelectedPlayer.Text = playerName;
                if (_selectedPlayerControl != null)
                {
                    panelSelectedPlayer.Controls.Remove(_selectedPlayerControl);
                    _selectedPlayerControl.Dispose();
                    _selectedPlayerControl = null;
                }
                return;
            }

            // Hide label and show PlayerUserControl
            labelSelectedPlayer.Visible = false;

            // Create or update the PlayerUserControl
            if (_selectedPlayerControl == null)
            {
                _selectedPlayerControl = new PlayerUserControl();
                _selectedPlayerControl.Dock = DockStyle.Fill;
                panelSelectedPlayer.Controls.Add(_selectedPlayerControl);
            }

            _selectedPlayerControl.SetPlayer(player);
        }

        #endregion
    }
}
