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
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsProvider _settings;
        private readonly IRankingsService _rankingsService;

        private readonly string _championship;
        private readonly string? _favouriteTeamFifaCode;
        private int _currentPrintPage;
        private int _currentPrintRow;
        private Dictionary<string, Player> _playerLookup = new();
        private PlayerUserControl? _selectedPlayerControl;

        public RankingsForm()
        {
            InitializeComponent();

            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;
            _rankingsService = new RankingsService();

            _championship = _settings.SelectedChampionship;
            _favouriteTeamFifaCode = _settings.FavouriteTeamFifaCode;
        }

        private async void RankingsForm_Load(object sender, EventArgs e)
        {
            ApplyLocalization();
            await LoadRankingsAsync();
        }

        private void ApplyLocalization()
        {
            Text = Translations.StringRankings;
            tabPageGoalScorers.Text = Translations.StringGoalScorers;
            tabPageYellowCards.Text = Translations.StringYellowCards;
            tabPageAttendance.Text = Translations.StringAttendance;
            buttonPrint.Text = Translations.StringPrintExport;

            labelFavouriteTeam.Text = !string.IsNullOrEmpty(_favouriteTeamFifaCode)
                ? $"{Translations.StringRankings}: {_favouriteTeamFifaCode}"
                : Translations.StringRankings;
        }

        #region Data Loading

        private async Task LoadRankingsAsync()
        {
            try
            {
                var matches = await LoadMatchesAsync();

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

                _playerLookup = _rankingsService.BuildPlayerLookup(matches);

                DisplayGoalScorersRanking(_rankingsService.GetGoalScorers(matches));
                DisplayYellowCardsRanking(_rankingsService.GetYellowCardRecipients(matches));
                DisplayAttendanceRanking(_rankingsService.GetAttendanceRanking(matches));
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

        private async Task<List<Match>> LoadMatchesAsync()
        {
            if (!string.IsNullOrEmpty(_favouriteTeamFifaCode))
            {
                var matches = await _repository.GetCountryMatchesAsync(_championship, _favouriteTeamFifaCode);
                return matches ?? new List<Match>();
            }
            else
            {
                var matches = await _repository.GetAllMatchesAsync(_championship);
                return matches ?? new List<Match>();
            }
        }

        #endregion

        #region Display Rankings

        private void DisplayGoalScorersRanking(List<PlayerStat> goalScorers)
        {
            SetupPlayerStatsGrid(dataGridViewGoalScorers, Translations.StringGoals);

            int rank = 1;
            foreach (var scorer in goalScorers)
            {
                var image = GetPlayerImage(scorer.PlayerName);
                dataGridViewGoalScorers.Rows.Add(rank++, image, scorer.PlayerName, scorer.Count);
            }
        }

        private void DisplayYellowCardsRanking(List<PlayerStat> yellowCards)
        {
            SetupPlayerStatsGrid(dataGridViewYellowCards, Translations.StringCards);

            int rank = 1;
            foreach (var card in yellowCards)
            {
                var image = GetPlayerImage(card.PlayerName);
                dataGridViewYellowCards.Rows.Add(rank++, image, card.PlayerName, card.Count);
            }
        }

        private void SetupPlayerStatsGrid(DataGridView grid, string countColumnHeader)
        {
            grid.DataSource = null;
            grid.Columns.Clear();
            grid.Rows.Clear();

            grid.Columns.Add("Rank", "#");
            grid.Columns["Rank"].Width = 40;

            var imageCol = new DataGridViewImageColumn
            {
                Name = "Image",
                HeaderText = "",
                Width = 40,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            grid.Columns.Add(imageCol);

            grid.Columns.Add("Player", Translations.StringPlayer);
            grid.Columns["Player"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            grid.Columns.Add("Count", countColumnHeader);
            grid.Columns["Count"].Width = 60;

            grid.RowTemplate.Height = 36;
        }

        private void DisplayAttendanceRanking(List<AttendanceRecord> attendanceRecords)
        {
            var attendanceList = attendanceRecords
                .Select((a, index) => new
                {
                    Rank = index + 1,
                    a.Location,
                    a.Attendance,
                    Match = a.MatchDescription
                })
                .ToList();

            dataGridViewAttendance.DataSource = attendanceList;

            if (attendanceList.Count > 0 && dataGridViewAttendance.Columns.Count > 0)
            {
                ConfigureAttendanceColumns();
            }
        }

        private void ConfigureAttendanceColumns()
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
            catch { }
        }

        #endregion

        #region Player Images

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
            int size = 32;
            var bitmap = new Bitmap(size, size);

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var (r, gb, b) = PlayerImageHelper.GetPlayerColorRgb(playerName);
            Color bgColor = Color.FromArgb(r, gb, b);
            using var brush = new SolidBrush(bgColor);
            g.FillEllipse(brush, 0, 0, size - 1, size - 1);

            string initials = PlayerImageHelper.GetInitials(playerName);
            using var font = new Font("Segoe UI", 10, FontStyle.Bold);
            using var textBrush = new SolidBrush(Color.White);

            var textSize = g.MeasureString(initials, font);
            float x = (size - textSize.Width) / 2;
            float y = (size - textSize.Height) / 2;
            g.DrawString(initials, font, textBrush, x, y);

            return bitmap;
        }

        #endregion

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

            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float pageWidth = e.MarginBounds.Width;
            float pageHeight = e.MarginBounds.Height;

            using var titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
            using var headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
            using var cellFont = new Font("Segoe UI", 10);

            float yPos = topMargin;
            float rowHeight = 25f;

            if (_currentPrintPage == 0 && _currentPrintRow == 0)
            {
                string title = GetPrintTitle();
                e.Graphics.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                yPos += titleFont.GetHeight(e.Graphics) + 20;

                string dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                e.Graphics.DrawString(dateStr, cellFont, Brushes.Gray, leftMargin, yPos);
                yPos += cellFont.GetHeight(e.Graphics) + 20;
            }

            int visibleColumns = grid.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            float colWidth = pageWidth / visibleColumns;

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

            using var pen = new Pen(Color.Black, 1);
            e.Graphics.DrawLine(pen, leftMargin, yPos, leftMargin + pageWidth, yPos);
            yPos += 5;

            while (_currentPrintRow < grid.Rows.Count && yPos + rowHeight < topMargin + pageHeight)
            {
                var row = grid.Rows[_currentPrintRow];
                xPos = leftMargin;

                if (_currentPrintRow % 2 == 1)
                {
                    using var altBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
                    e.Graphics.FillRectangle(altBrush, leftMargin, yPos, pageWidth, rowHeight);
                }

                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (!col.Visible) continue;

                    var cellValue = row.Cells[col.Index].Value;

                    if (col is DataGridViewImageColumn && cellValue is Image img)
                    {
                        float imgSize = rowHeight - 4;
                        e.Graphics.DrawImage(img, xPos + 2, yPos + 2, imgSize, imgSize);
                    }
                    else
                    {
                        string textValue = cellValue?.ToString() ?? "";

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
            if (!_playerLookup.TryGetValue(playerName, out var player))
            {
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

            labelSelectedPlayer.Visible = false;

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
