using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DataLayer;
using Utils.Helpers;

namespace WindowsForms.UserControls
{
    public partial class PlayerUserControl : UserControl
    {
        // Backing fields
        private string _playerName = string.Empty;
        private int _shirtNumber;
        private string _position = string.Empty;
        private bool _isCaptain;
        private bool _isFavourite;
        private bool _isSelected;
        private Image? _playerImage;

        // Colors for visual states
        private static readonly Color NormalBackColor = SystemColors.Control;
        private static readonly Color HoverBackColor = Color.FromArgb(230, 240, 250);
        private static readonly Color SelectedBackColor = Color.FromArgb(200, 220, 240);
        private static readonly Color FavouriteStarColor = Color.Gold;
        private static readonly Color NonFavouriteStarColor = Color.LightGray;

        // Events
        public event EventHandler? PlayerClicked;
        public event EventHandler? PlayerDoubleClicked;
        public event EventHandler? FavouriteToggled;

        #region Properties

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                UpdateDisplay();
            }
        }

        public int ShirtNumber
        {
            get => _shirtNumber;
            set
            {
                _shirtNumber = value;
                UpdateDisplay();
            }
        }

        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateDisplay();
            }
        }

        public bool IsCaptain
        {
            get => _isCaptain;
            set
            {
                _isCaptain = value;
                UpdateDisplay();
            }
        }

        public bool IsFavourite
        {
            get => _isFavourite;
            set
            {
                _isFavourite = value;
                UpdateFavouriteDisplay();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                UpdateSelectionDisplay();
            }
        }

        public string PlayerIdentifier => Constant.GeneratePlayerIdentifier(_playerName, _shirtNumber);

        public Image? PlayerImage
        {
            get => _playerImage;
            set
            {
                _playerImage = value;
                if (pictureBoxPlayer != null)
                {
                    pictureBoxPlayer.Image = _playerImage;
                }
            }
        }

        #endregion

        public PlayerUserControl()
        {
            InitializeComponent();
            SetupEventHandlers();
            UpdateSelectionDisplay();
        }

        /// <summary>
        /// Initialize the control with player data from the DataLayer Player model
        /// </summary>
        public void SetPlayer(DataLayer.Models.Player player)
        {
            _playerName = player.Name;
            _shirtNumber = player.ShirtNumber;
            _position = player.Position;
            _isCaptain = player.Captain;

            // Check if this player is a favourite
            _isFavourite = Settings.Instance.IsFavouritePlayer(PlayerIdentifier);

            UpdateDisplay();
            UpdateFavouriteDisplay();
            LoadPlayerImage();
        }

        /// <summary>
        /// Load the player's image from settings or create dynamic placeholder with initials
        /// </summary>
        public void LoadPlayerImage()
        {
            try
            {
                // Try to get image path from settings (custom assigned image)
                string? imagePath = Settings.Instance.GetPlayerImagePath(PlayerIdentifier);

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // Load custom image without locking the file
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        PlayerImage = Image.FromStream(stream);
                    }
                }
                else
                {
                    // No custom image assigned - create dynamic placeholder with player initials
                    PlayerImage = CreatePlaceholderImage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading player image: {ex.Message}");
                PlayerImage = CreatePlaceholderImage();
            }
        }

        /// <summary>
        /// Creates a placeholder image with player initials displayed prominently
        /// </summary>
        private Image CreatePlaceholderImage()
        {
            int size = 50;
            Bitmap placeholder = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(placeholder))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // Generate a consistent color based on player name using shared helper
                var (r, green, b) = PlayerImageHelper.GetPlayerColorRgb(_playerName);
                Color bgColor = Color.FromArgb(r, green, b);

                // Fill circular background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillEllipse(brush, 1, 1, size - 2, size - 2);
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    g.DrawEllipse(pen, 1, 1, size - 2, size - 2);
                }

                // Draw initials in center using shared helper
                if (!string.IsNullOrEmpty(_playerName))
                {
                    string initials = PlayerImageHelper.GetInitials(_playerName);
                    using (Font font = new Font("Segoe UI", 16, FontStyle.Bold))
                    {
                        SizeF textSize = g.MeasureString(initials, font);
                        float textX = (size - textSize.Width) / 2;
                        float textY = (size - textSize.Height) / 2;
                        g.DrawString(initials, font, Brushes.White, textX, textY);
                    }
                }
            }

            return placeholder;
        }

        private void SetupEventHandlers()
        {
            // Wire up events for all child controls to bubble up to parent
            // This ensures mouse events work regardless of which part of the control is clicked

            this.Click += OnControlClicked;
            this.DoubleClick += OnControlDoubleClicked;
            this.MouseEnter += OnMouseEnterControl;
            this.MouseLeave += OnMouseLeaveControl;

            // Wire up all child controls for click, double-click, and mouse tracking
            Control[] childControls = { pictureBoxPlayer, labelPlayerName, labelPosition, panelInfo };

            foreach (var child in childControls)
            {
                child.Click += OnControlClicked;
                child.DoubleClick += OnControlDoubleClicked;
                child.MouseEnter += OnMouseEnterControl;
                child.MouseLeave += OnMouseLeaveControl;

                // Forward mouse events for drag detection
                child.MouseDown += ForwardMouseDown;
                child.MouseMove += ForwardMouseMove;
                child.MouseUp += ForwardMouseUp;
            }

            // Star has special click handling (toggles favourite)
            labelStar.Click += OnStarClicked;
            labelStar.MouseEnter += OnMouseEnterControl;
            labelStar.MouseLeave += OnMouseLeaveControl;
            // Star also needs mouse forwarding for drag support
            labelStar.MouseDown += ForwardMouseDown;
            labelStar.MouseMove += ForwardMouseMove;
            labelStar.MouseUp += ForwardMouseUp;
        }

        // Forward mouse events from child controls to this control's event handlers
        private void ForwardMouseDown(object? sender, MouseEventArgs e)
        {
            // Convert the mouse position from child control coordinates to this control's coordinates
            if (sender is Control child)
            {
                Point childLocation = child.Location;
                var newArgs = new MouseEventArgs(e.Button, e.Clicks, e.X + childLocation.X, e.Y + childLocation.Y, e.Delta);
                OnMouseDown(newArgs);
            }
        }

        private void ForwardMouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is Control child)
            {
                Point childLocation = child.Location;
                var newArgs = new MouseEventArgs(e.Button, e.Clicks, e.X + childLocation.X, e.Y + childLocation.Y, e.Delta);
                OnMouseMove(newArgs);
            }
        }

        private void ForwardMouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is Control child)
            {
                Point childLocation = child.Location;
                var newArgs = new MouseEventArgs(e.Button, e.Clicks, e.X + childLocation.X, e.Y + childLocation.Y, e.Delta);
                OnMouseUp(newArgs);
            }
        }

        private void UpdateDisplay()
        {
            if (labelPlayerName == null || labelPosition == null) return;

            string captainIndicator = _isCaptain ? " (C)" : "";
            labelPlayerName.Text = $"#{_shirtNumber} {_playerName}{captainIndicator}";
            labelPosition.Text = _position;
        }

        private void UpdateFavouriteDisplay()
        {
            if (labelStar == null) return;

            labelStar.ForeColor = _isFavourite ? FavouriteStarColor : NonFavouriteStarColor;
            labelStar.Text = _isFavourite ? "\u2605" : "\u2606"; // Filled star vs outline star
        }

        private void UpdateSelectionDisplay()
        {
            BackColor = _isSelected ? SelectedBackColor : NormalBackColor;
        }

        #region Event Handlers

        private void OnControlClicked(object? sender, EventArgs e)
        {
            // Just raise the event - let the parent form handle selection logic
            // This avoids conflicts with form-level multi-select handling
            PlayerClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnControlDoubleClicked(object? sender, EventArgs e)
        {
            PlayerDoubleClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnStarClicked(object? sender, EventArgs e)
        {
            // Toggle favourite status
            IsFavourite = !IsFavourite;
            FavouriteToggled?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseEnterControl(object? sender, EventArgs e)
        {
            if (!_isSelected)
            {
                BackColor = HoverBackColor;
            }
        }

        private void OnMouseLeaveControl(object? sender, EventArgs e)
        {
            if (!_isSelected)
            {
                BackColor = NormalBackColor;
            }
        }

        #endregion

        #region Drag and Drop Support

        // Note: Drag operation is NOT started here.
        // The parent form (FavoritePlayersForm) handles drag via PlayerControl_MouseDown
        // to properly support multi-select drag operations.

        #endregion
    }
}
