using DataLayer;
using DataLayer.Interfaces;
using DataLayer.Models;
using DataLayer.Repositories;
using Utils;
using WindowsForms.UserControls;

namespace WindowsForms
{
    public partial class FavoritePlayersForm : Form
    {
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        private readonly string _championship;
        private readonly string? _favouriteTeamFifaCode;
        private List<Player> _allPlayers = new();
        private List<PlayerUserControl> _selectedControls = new();
        private const int MaxFavourites = 3;

        private ContextMenuStrip _contextMenu = new();
        private ToolStripMenuItem _menuAddToFavourites = new();
        private ToolStripMenuItem _menuRemoveFromFavourites = new();

        private Point _dragStartPoint;
        private bool _isDragging;
        private PlayerUserControl? _dragSourceControl;
        private const int DragThreshold = 5;

        public FavoritePlayersForm()
        {
            InitializeComponent();

            _repository = WorldCupRepository.Instance;
            _settings = Settings.Instance;

            _championship = _settings.SelectedChampionship;
            _favouriteTeamFifaCode = _settings.FavouriteTeamFifaCode;

            SetupContextMenu();
            SetupDragDrop();
        }

        private async void FavoritePlayersForm_Load(object sender, EventArgs e)
        {
            ApplyLocalization();

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

        private void ApplyLocalization()
        {
            Text = Translations.StringFavouritePlayers;
            labelFavourites.Text = Translations.StringFavouritePlayers;
            labelOthers.Text = Translations.StringOtherPlayers;
            buttonSave.Text = Translations.StringSave;
            _menuAddToFavourites.Text = Translations.StringAddToFavourites;
            _menuRemoveFromFavourites.Text = Translations.StringRemoveFromFavourites;
        }

        private void SetupContextMenu()
        {
            _menuAddToFavourites.Click += MenuAddToFavourites_Click;
            _menuRemoveFromFavourites.Click += MenuRemoveFromFavourites_Click;

            _contextMenu.Items.Add(_menuAddToFavourites);
            _contextMenu.Items.Add(_menuRemoveFromFavourites);
        }

        private void SetupDragDrop()
        {
            panelFavourites.AllowDrop = true;
            panelOthers.AllowDrop = true;

            panelFavourites.DragEnter += Panel_DragEnter;
            panelFavourites.DragDrop += PanelFavourites_DragDrop;

            panelOthers.DragEnter += Panel_DragEnter;
            panelOthers.DragDrop += PanelOthers_DragDrop;
        }

        #region Player Loading

        private async Task LoadPlayersAsync()
        {
            try
            {
                labelStatus.Text = Translations.StringLoading;
                labelStatus.Visible = true;

                var matches = await _repository.GetCountryMatchesAsync(_championship, _favouriteTeamFifaCode!);

                if (matches == null || matches.Count == 0)
                {
                    MessageBox.Show(
                        Translations.StringNoMatchesFound,
                        Translations.StringWarning,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    labelStatus.Visible = false;
                    return;
                }

                var firstMatch = matches[0];
                bool isHomeTeam = firstMatch.HomeTeam.Code == _favouriteTeamFifaCode;
                var teamStats = isHomeTeam ? firstMatch.HomeTeamStatistics : firstMatch.AwayTeamStatistics;

                _allPlayers = new List<Player>();
                if (teamStats.StartingEleven != null)
                    _allPlayers.AddRange(teamStats.StartingEleven);
                if (teamStats.Substitutes != null)
                    _allPlayers.AddRange(teamStats.Substitutes);

                _allPlayers = _allPlayers.OrderBy(p => p.ShirtNumber).ToList();

                PopulatePanels();
                labelStatus.Visible = false;
            }
            catch (Exception)
            {
                labelStatus.Visible = false;
                MessageBox.Show(
                    Translations.StringErrorLoadingData,
                    Translations.StringError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void PopulatePanels()
        {
            panelFavourites.Controls.Clear();
            panelOthers.Controls.Clear();
            _selectedControls.Clear();

            foreach (var player in _allPlayers)
            {
                var control = CreatePlayerControl(player);
                string identifier = Constant.GeneratePlayerIdentifier(player.Name, player.ShirtNumber);

                if (_settings.IsFavouritePlayer(identifier))
                {
                    control.IsFavourite = true;
                    panelFavourites.Controls.Add(control);
                }
                else
                {
                    panelOthers.Controls.Add(control);
                }
            }

            UpdateFavouritesCount();
        }

        private PlayerUserControl CreatePlayerControl(Player player)
        {
            var control = new PlayerUserControl();
            control.SetPlayer(player);
            control.Margin = new Padding(5);

            control.PlayerClicked += PlayerControl_Clicked;
            control.PlayerDoubleClicked += PlayerControl_DoubleClicked;
            control.FavouriteToggled += PlayerControl_FavouriteToggled;

            control.MouseDown += PlayerControl_MouseDown;
            control.MouseMove += PlayerControl_MouseMove;
            control.MouseUp += PlayerControl_MouseUp;

            control.ContextMenuStrip = _contextMenu;

            return control;
        }

        #endregion

        #region Player Control Events

        private void PlayerControl_Clicked(object? sender, EventArgs e)
        {
            if (sender is not PlayerUserControl control) return;

            if (ModifierKeys.HasFlag(Keys.Control))
            {
                if (control.IsSelected)
                {
                    control.IsSelected = false;
                    _selectedControls.Remove(control);
                }
                else
                {
                    control.IsSelected = true;
                    _selectedControls.Add(control);
                }
            }
            else
            {
                ClearSelection();
                control.IsSelected = true;
                _selectedControls.Add(control);
            }

            UpdateContextMenuState();
        }

        private void PlayerControl_DoubleClicked(object? sender, EventArgs e)
        {
            if (sender is not PlayerUserControl control) return;
            ToggleFavourite(control);
        }

        private void PlayerControl_FavouriteToggled(object? sender, EventArgs e)
        {
            if (sender is not PlayerUserControl control) return;
            ToggleFavourite(control);
        }

        private void PlayerControl_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not PlayerUserControl control) return;

            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
                _dragSourceControl = control;
                _isDragging = false;
            }
        }

        private void PlayerControl_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not PlayerUserControl control) return;
            if (e.Button != MouseButtons.Left || _dragSourceControl == null) return;

            if (!_isDragging)
            {
                int deltaX = Math.Abs(e.X - _dragStartPoint.X);
                int deltaY = Math.Abs(e.Y - _dragStartPoint.Y);

                if (deltaX > DragThreshold || deltaY > DragThreshold)
                {
                    _isDragging = true;

                    if (!_dragSourceControl.IsSelected)
                    {
                        ClearSelection();
                        _dragSourceControl.IsSelected = true;
                        _selectedControls.Add(_dragSourceControl);
                    }

                    if (_selectedControls.Count > 0)
                    {
                        _dragSourceControl.DoDragDrop(_selectedControls.ToList(), DragDropEffects.Move);
                    }

                    _dragSourceControl = null;
                    _isDragging = false;
                }
            }
        }

        private void PlayerControl_MouseUp(object? sender, MouseEventArgs e)
        {
            _dragSourceControl = null;
            _isDragging = false;
        }

        #endregion

        #region Drag and Drop

        private void Panel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(typeof(List<PlayerUserControl>)) == true)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PanelFavourites_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(List<PlayerUserControl>)) is not List<PlayerUserControl> controls) return;

            int currentFavourites = panelFavourites.Controls.Count;
            int toAdd = controls.Count(c => c.Parent != panelFavourites);

            if (currentFavourites + toAdd > MaxFavourites)
            {
                MessageBox.Show(
                    Translations.StringMaxFavouritesReached,
                    Translations.StringFavouritePlayers,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            foreach (var control in controls.OrderBy(c => c.ShirtNumber))
            {
                if (control.Parent != panelFavourites)
                {
                    control.Parent?.Controls.Remove(control);
                    control.IsFavourite = true;
                    InsertControlInOrder(panelFavourites, control);
                }
            }

            UpdateFavouritesCount();
            ClearSelection();
        }

        private void PanelOthers_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(List<PlayerUserControl>)) is not List<PlayerUserControl> controls) return;

            foreach (var control in controls.OrderBy(c => c.ShirtNumber))
            {
                if (control.Parent != panelOthers)
                {
                    control.Parent?.Controls.Remove(control);
                    control.IsFavourite = false;
                    InsertControlInOrder(panelOthers, control);
                }
            }

            UpdateFavouritesCount();
            ClearSelection();
        }

        #endregion

        #region Context Menu

        private void UpdateContextMenuState()
        {
            if (_selectedControls.Count == 0)
            {
                _menuAddToFavourites.Enabled = false;
                _menuRemoveFromFavourites.Enabled = false;
                return;
            }

            bool anyInOthers = _selectedControls.Any(c => c.Parent == panelOthers);
            bool anyInFavourites = _selectedControls.Any(c => c.Parent == panelFavourites);

            _menuAddToFavourites.Enabled = anyInOthers;
            _menuRemoveFromFavourites.Enabled = anyInFavourites;
        }

        private void MenuAddToFavourites_Click(object? sender, EventArgs e)
        {
            var controlsToMove = _selectedControls.Where(c => c.Parent == panelOthers).ToList();

            if (controlsToMove.Count == 0) return;

            int currentFavourites = panelFavourites.Controls.Count;
            if (currentFavourites + controlsToMove.Count > MaxFavourites)
            {
                MessageBox.Show(
                    Translations.StringMaxFavouritesReached,
                    Translations.StringFavouritePlayers,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            foreach (var control in controlsToMove.OrderBy(c => c.ShirtNumber))
            {
                panelOthers.Controls.Remove(control);
                control.IsFavourite = true;
                InsertControlInOrder(panelFavourites, control);
            }

            UpdateFavouritesCount();
            ClearSelection();
        }

        private void MenuRemoveFromFavourites_Click(object? sender, EventArgs e)
        {
            var controlsToMove = _selectedControls.Where(c => c.Parent == panelFavourites).ToList();

            foreach (var control in controlsToMove.OrderBy(c => c.ShirtNumber))
            {
                panelFavourites.Controls.Remove(control);
                control.IsFavourite = false;
                InsertControlInOrder(panelOthers, control);
            }

            UpdateFavouritesCount();
            ClearSelection();
        }

        #endregion

        #region Helper Methods

        private void InsertControlInOrder(FlowLayoutPanel panel, PlayerUserControl control)
        {
            int shirtNumber = control.ShirtNumber;
            int insertIndex = 0;

            for (int i = 0; i < panel.Controls.Count; i++)
            {
                if (panel.Controls[i] is PlayerUserControl existingControl)
                {
                    if (existingControl.ShirtNumber > shirtNumber)
                    {
                        break;
                    }
                    insertIndex = i + 1;
                }
            }

            panel.Controls.Add(control);
            panel.Controls.SetChildIndex(control, insertIndex);
        }

        private void ToggleFavourite(PlayerUserControl control)
        {
            if (control.Parent == panelFavourites)
            {
                panelFavourites.Controls.Remove(control);
                control.IsFavourite = false;
                InsertControlInOrder(panelOthers, control);
            }
            else if (control.Parent == panelOthers)
            {
                if (panelFavourites.Controls.Count >= MaxFavourites)
                {
                    MessageBox.Show(
                        Translations.StringMaxFavouritesReached,
                        Translations.StringFavouritePlayers,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    control.IsFavourite = false;
                    return;
                }

                panelOthers.Controls.Remove(control);
                control.IsFavourite = true;
                InsertControlInOrder(panelFavourites, control);
            }

            UpdateFavouritesCount();
        }

        private void ClearSelection()
        {
            foreach (var control in _selectedControls)
            {
                control.IsSelected = false;
            }
            _selectedControls.Clear();
            UpdateContextMenuState();
        }

        private void UpdateFavouritesCount()
        {
            int count = panelFavourites.Controls.Count;
            labelFavouritesCount.Text = $"({count}/{MaxFavourites})";
            buttonSave.Enabled = true;
        }

        #endregion

        #region Save/Close

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                buttonSave.Enabled = false;
                buttonSave.Text = Translations.StringLoading;

                _settings.ClearFavouritePlayers();

                foreach (Control control in panelFavourites.Controls)
                {
                    if (control is PlayerUserControl playerControl)
                    {
                        _settings.AddFavouritePlayer(playerControl.PlayerIdentifier);
                    }
                }

                await _settings.SaveSettingsAsync();

                MessageBox.Show(
                    Translations.StringFavouritesSaved,
                    Translations.StringSuccess,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                buttonSave.Text = Translations.StringSave;
                buttonSave.Enabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Translations.StringErrorSavingSettings,
                    Translations.StringError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                buttonSave.Text = Translations.StringSave;
                buttonSave.Enabled = true;
            }
        }

        private void FavoritePlayersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var currentFavourites = new HashSet<string>();
            foreach (Control control in panelFavourites.Controls)
            {
                if (control is PlayerUserControl playerControl)
                {
                    currentFavourites.Add(playerControl.PlayerIdentifier);
                }
            }

            var savedFavourites = new HashSet<string>(_settings.FavouritePlayersList);

            if (!currentFavourites.SetEquals(savedFavourites))
            {
                var result = MessageBox.Show(
                    Translations.StringUnsavedChanges,
                    Translations.StringConfirm,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    ButtonSave_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #region Keyboard Shortcuts

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    ButtonSave_Click(this, EventArgs.Empty);
                    return true;

                case Keys.Escape:
                    Close();
                    return true;

                case Keys.Control | Keys.A:
                    SelectAllInPanel();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void SelectAllInPanel()
        {
            FlowLayoutPanel? targetPanel = null;

            if (_selectedControls.Count > 0)
            {
                targetPanel = _selectedControls[0].Parent as FlowLayoutPanel;
            }
            else
            {
                targetPanel = panelOthers;
            }

            if (targetPanel == null) return;

            ClearSelection();

            foreach (Control control in targetPanel.Controls)
            {
                if (control is PlayerUserControl playerControl)
                {
                    playerControl.IsSelected = true;
                    _selectedControls.Add(playerControl);
                }
            }

            UpdateContextMenuState();
        }

        #endregion
    }
}
