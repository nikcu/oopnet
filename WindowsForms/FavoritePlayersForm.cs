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
        // Dependencies (using interfaces for DIP)
        private readonly IWorldCupRepository _repository;
        private readonly ISettingsService _settings;

        private readonly string _championship;
        private readonly string? _favouriteTeamFifaCode;
        private List<Player> _allPlayers = new();
        private List<PlayerUserControl> _selectedControls = new();
        private const int MaxFavourites = 3;

        // Context menu
        private ContextMenuStrip _contextMenu = new();
        private ToolStripMenuItem _menuAddToFavourites = new();
        private ToolStripMenuItem _menuRemoveFromFavourites = new();

        // Drag detection
        private Point _dragStartPoint;
        private bool _isDragging;
        private PlayerUserControl? _dragSourceControl;
        private const int DragThreshold = 5; // Pixels to move before drag starts

        public FavoritePlayersForm()
        {
            InitializeComponent();

            // Initialize dependencies
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
            // Enable drag-drop for both panels
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
                    labelStatus.Visible = false;
                    return;
                }

                // Get players from first match (starting_eleven + substitutes)
                var firstMatch = matches[0];

                // Determine if favourite team is home or away
                bool isHomeTeam = firstMatch.HomeTeam.Code == _favouriteTeamFifaCode;
                var teamStats = isHomeTeam ? firstMatch.HomeTeamStatistics : firstMatch.AwayTeamStatistics;

                _allPlayers = new List<Player>();
                if (teamStats.StartingEleven != null)
                    _allPlayers.AddRange(teamStats.StartingEleven);
                if (teamStats.Substitutes != null)
                    _allPlayers.AddRange(teamStats.Substitutes);

                // Sort by shirt number
                _allPlayers = _allPlayers.OrderBy(p => p.ShirtNumber).ToList();

                Console.WriteLine($"Loaded {_allPlayers.Count} players");

                // Populate panels
                PopulatePanels();

                labelStatus.Visible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading players: {ex.Message}");
                labelStatus.Visible = false;
                MessageBox.Show(
                    $"Failed to load players.\n\n{ex.Message}",
                    "Error",
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

                // Check if this player is already a favourite
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

            // Wire up events
            control.PlayerClicked += PlayerControl_Clicked;
            control.PlayerDoubleClicked += PlayerControl_DoubleClicked;
            control.FavouriteToggled += PlayerControl_FavouriteToggled;

            // Drag detection events
            control.MouseDown += PlayerControl_MouseDown;
            control.MouseMove += PlayerControl_MouseMove;
            control.MouseUp += PlayerControl_MouseUp;

            // Context menu
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
                // Ctrl+Click: Toggle selection (multi-select)
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
                // Regular click: Select only this one
                ClearSelection();
                control.IsSelected = true;
                _selectedControls.Add(control);
            }

            UpdateContextMenuState();
        }

        private void PlayerControl_DoubleClicked(object? sender, EventArgs e)
        {
            if (sender is not PlayerUserControl control) return;

            // Double-click toggles favourite status
            ToggleFavourite(control);
        }

        private void PlayerControl_FavouriteToggled(object? sender, EventArgs e)
        {
            if (sender is not PlayerUserControl control) return;

            // Star was clicked - toggle favourite
            ToggleFavourite(control);
        }

        private void PlayerControl_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not PlayerUserControl control) return;

            if (e.Button == MouseButtons.Left)
            {
                // Record the starting point for drag detection
                _dragStartPoint = e.Location;
                _dragSourceControl = control;
                _isDragging = false;
            }
        }

        private void PlayerControl_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not PlayerUserControl control) return;

            // Only process if left button is held and we have a drag source
            if (e.Button != MouseButtons.Left || _dragSourceControl == null) return;

            // Check if we've moved beyond the drag threshold
            if (!_isDragging)
            {
                int deltaX = Math.Abs(e.X - _dragStartPoint.X);
                int deltaY = Math.Abs(e.Y - _dragStartPoint.Y);

                if (deltaX > DragThreshold || deltaY > DragThreshold)
                {
                    _isDragging = true;

                    // If the drag source isn't selected, select only it
                    if (!_dragSourceControl.IsSelected)
                    {
                        ClearSelection();
                        _dragSourceControl.IsSelected = true;
                        _selectedControls.Add(_dragSourceControl);
                    }

                    // Start the drag operation with all selected controls
                    if (_selectedControls.Count > 0)
                    {
                        _dragSourceControl.DoDragDrop(_selectedControls.ToList(), DragDropEffects.Move);
                    }

                    // Reset drag state after drag completes
                    _dragSourceControl = null;
                    _isDragging = false;
                }
            }
        }

        private void PlayerControl_MouseUp(object? sender, MouseEventArgs e)
        {
            // Reset drag state on mouse up
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

            // Check if we can add more favourites
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

            // Move controls to favourites panel (maintain order)
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

            // Move controls to others panel (maintain order)
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

            // Check if any selected are in Others panel (can add to favourites)
            bool anyInOthers = _selectedControls.Any(c => c.Parent == panelOthers);
            // Check if any selected are in Favourites panel (can remove from favourites)
            bool anyInFavourites = _selectedControls.Any(c => c.Parent == panelFavourites);

            _menuAddToFavourites.Enabled = anyInOthers;
            _menuRemoveFromFavourites.Enabled = anyInFavourites;
        }

        private void MenuAddToFavourites_Click(object? sender, EventArgs e)
        {
            var controlsToMove = _selectedControls.Where(c => c.Parent == panelOthers).ToList();

            if (controlsToMove.Count == 0) return;

            // Check limit
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

            // Move controls maintaining order
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

            // Move controls maintaining order
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

        /// <summary>
        /// Inserts a player control into a panel at the correct position to maintain shirt number order
        /// </summary>
        private void InsertControlInOrder(FlowLayoutPanel panel, PlayerUserControl control)
        {
            int shirtNumber = control.ShirtNumber;
            int insertIndex = 0;

            // Find the correct position based on shirt number
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

            // Add the control and set its position
            panel.Controls.Add(control);
            panel.Controls.SetChildIndex(control, insertIndex);
        }

        private void ToggleFavourite(PlayerUserControl control)
        {
            if (control.Parent == panelFavourites)
            {
                // Remove from favourites (insert in correct position in Others)
                panelFavourites.Controls.Remove(control);
                control.IsFavourite = false;
                InsertControlInOrder(panelOthers, control);
            }
            else if (control.Parent == panelOthers)
            {
                // Add to favourites (check limit)
                if (panelFavourites.Controls.Count >= MaxFavourites)
                {
                    MessageBox.Show(
                        Translations.StringMaxFavouritesReached,
                        Translations.StringFavouritePlayers,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    // Reset the star state since we can't add
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

            // Enable/disable save button
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

                // Clear existing favourites
                _settings.ClearFavouritePlayers();

                // Add current favourites
                foreach (Control control in panelFavourites.Controls)
                {
                    if (control is PlayerUserControl playerControl)
                    {
                        _settings.AddFavouritePlayer(playerControl.PlayerIdentifier);
                        Console.WriteLine($"Saved favourite: {playerControl.PlayerIdentifier}");
                    }
                }

                // Save settings
                await _settings.SaveSettingsAsync();

                MessageBox.Show(
                    $"Saved {panelFavourites.Controls.Count} favourite player(s).",
                    Translations.StringFavouritePlayers,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                buttonSave.Text = Translations.StringSave;
                buttonSave.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving favourites: {ex.Message}");
                MessageBox.Show(
                    $"Failed to save favourites.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                buttonSave.Text = Translations.StringSave;
                buttonSave.Enabled = true;
            }
        }

        private void FavoritePlayersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if there are unsaved changes
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
                    "You have unsaved changes. Do you want to save before closing?",
                    Translations.StringFavouritePlayers,
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
                    // Select all in current panel
                    SelectAllInPanel();
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void SelectAllInPanel()
        {
            // Determine which panel has focus based on last selected control
            FlowLayoutPanel? targetPanel = null;

            if (_selectedControls.Count > 0)
            {
                targetPanel = _selectedControls[0].Parent as FlowLayoutPanel;
            }
            else
            {
                // Default to others panel
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
