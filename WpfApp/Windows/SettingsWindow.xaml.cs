using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using DataLayer;
using DataLayer.Interfaces;
using Utils;
using WpfComboBoxItem = System.Windows.Controls.ComboBoxItem;

namespace WpfApp.Windows
{
    public partial class SettingsWindow : Window
    {
        // Dependencies (using interfaces for DIP)
        private readonly ISettingsService _settings;

        private bool _settingsSaved = false;
        private bool _isInitializing = false;

        public SettingsWindow()
        {
            InitializeComponent();

            // Initialize dependencies
            _settings = Settings.Instance;

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            _isInitializing = true;
            try
            {
                // Apply current culture
                if (_settings.GetIsLoadedFromFile())
                {
                    try
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(_settings.SelectedLanguage);
                    }
                    catch (CultureNotFoundException)
                    {
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    }
                }

                ApplyLocalization();
                InitializeComboBoxes();
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private void ApplyLocalization()
        {
            Title = $"World Cup 2018 - {Translations.StringSettings}";
            labelLanguage.Content = Translations.StringLanguage;
            labelChampionship.Content = Translations.StringChampionship;
            labelDisplayMode.Content = Translations.StringDisplayMode;
            labelResolution.Content = Translations.StringResolution;
            buttonSave.Content = Translations.StringSave;
            buttonCancel.Content = Translations.StringCancel;
        }

        private void InitializeComboBoxes()
        {
            // Language ComboBox
            comboBoxLanguage.Items.Clear();
            comboBoxLanguage.Items.Add(new WpfComboBoxItem { Content = Translations.StringLangEnglish, Tag = "en" });
            comboBoxLanguage.Items.Add(new WpfComboBoxItem { Content = Translations.StringLangCroatian, Tag = "hr" });

            // Select current language
            foreach (WpfComboBoxItem item in comboBoxLanguage.Items)
            {
                if (item.Tag?.ToString() == _settings.SelectedLanguage)
                {
                    comboBoxLanguage.SelectedItem = item;
                    break;
                }
            }
            if (comboBoxLanguage.SelectedItem == null)
                comboBoxLanguage.SelectedIndex = 0;

            // Championship ComboBox
            comboBoxChampionship.Items.Clear();
            comboBoxChampionship.Items.Add(new WpfComboBoxItem { Content = Translations.StringMens, Tag = "m" });
            comboBoxChampionship.Items.Add(new WpfComboBoxItem { Content = Translations.StringWomens, Tag = "f" });

            // Select current championship
            foreach (WpfComboBoxItem item in comboBoxChampionship.Items)
            {
                if (item.Tag?.ToString() == _settings.SelectedChampionship)
                {
                    comboBoxChampionship.SelectedItem = item;
                    break;
                }
            }
            if (comboBoxChampionship.SelectedItem == null)
                comboBoxChampionship.SelectedIndex = 0;

            // Display Mode ComboBox
            comboBoxDisplayMode.Items.Clear();
            comboBoxDisplayMode.Items.Add(new WpfComboBoxItem { Content = Translations.StringFullscreen, Tag = "fullscreen" });
            comboBoxDisplayMode.Items.Add(new WpfComboBoxItem { Content = Translations.StringWindowed, Tag = "windowed" });

            // Select current display mode
            comboBoxDisplayMode.SelectedIndex = _settings.WpfIsFullscreen ? 0 : 1;
            comboBoxDisplayMode.SelectionChanged += ComboBoxDisplayMode_SelectionChanged;

            // Resolution ComboBox
            comboBoxResolution.Items.Clear();
            comboBoxResolution.Items.Add(new WpfComboBoxItem { Content = "1280 x 720", Tag = "1280x720" });
            comboBoxResolution.Items.Add(new WpfComboBoxItem { Content = "1366 x 768", Tag = "1366x768" });
            comboBoxResolution.Items.Add(new WpfComboBoxItem { Content = "1920 x 1080", Tag = "1920x1080" });
            comboBoxResolution.Items.Add(new WpfComboBoxItem { Content = "2560 x 1440", Tag = "2560x1440" });

            // Select current resolution
            string currentRes = $"{_settings.WpfResolutionWidth}x{_settings.WpfResolutionHeight}";
            bool found = false;
            foreach (WpfComboBoxItem item in comboBoxResolution.Items)
            {
                if (item.Tag?.ToString() == currentRes)
                {
                    comboBoxResolution.SelectedItem = item;
                    found = true;
                    break;
                }
            }
            if (!found)
                comboBoxResolution.SelectedIndex = 0;

            // Update resolution visibility based on display mode
            UpdateResolutionVisibility();
        }

        private void UpdateResolutionVisibility()
        {
            bool isWindowed = comboBoxDisplayMode.SelectedIndex == 1;
            labelResolution.Visibility = isWindowed ? Visibility.Visible : Visibility.Collapsed;
            comboBoxResolution.Visibility = isWindowed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ComboBoxDisplayMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateResolutionVisibility();
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;

            if (comboBoxLanguage.SelectedItem is WpfComboBoxItem item && item.Tag != null)
            {
                string lang = item.Tag.ToString()!;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
                _settings.SelectedLanguage = lang;

                _isInitializing = true;
                try
                {
                    ApplyLocalization();
                    InitializeComboBoxes();
                }
                finally
                {
                    _isInitializing = false;
                }
            }
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                buttonSave.IsEnabled = false;
                buttonSave.Content = Translations.StringLoading;

                // Save language
                if (comboBoxLanguage.SelectedItem is WpfComboBoxItem langItem)
                {
                    _settings.SelectedLanguage = langItem.Tag?.ToString() ?? "en";
                }

                // Save championship
                if (comboBoxChampionship.SelectedItem is WpfComboBoxItem champItem)
                {
                    _settings.SelectedChampionship = champItem.Tag?.ToString() ?? "m";
                }

                // Save display mode
                _settings.WpfIsFullscreen = comboBoxDisplayMode.SelectedIndex == 0;

                // Save resolution
                if (comboBoxResolution.SelectedItem is WpfComboBoxItem resItem && resItem.Tag != null)
                {
                    string[] parts = resItem.Tag.ToString()!.Split('x');
                    if (parts.Length == 2)
                    {
                        _settings.WpfResolutionWidth = int.Parse(parts[0]);
                        _settings.WpfResolutionHeight = int.Parse(parts[1]);
                    }
                }

                // Save to file
                await _settings.SaveSettingsAsync();

                _settingsSaved = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
                MessageBox.Show(
                    Translations.StringErrorSavingSettings,
                    Translations.StringError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                buttonSave.IsEnabled = true;
                buttonSave.Content = Translations.StringSave;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If settings weren't saved and this is initial setup, confirm closing
            if (!_settingsSaved && !_settings.GetIsLoadedFromFile())
            {
                var result = MessageBox.Show(
                    Translations.StringCloseConfirmation,
                    Translations.StringSettings,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
