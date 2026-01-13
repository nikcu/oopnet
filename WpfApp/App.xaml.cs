using System.Globalization;
using System.Windows;
using DataLayer;
using WpfApp.Windows;

namespace WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize settings and apply language (shared with Windows Forms)
        var settings = Settings.Instance;

        Console.WriteLine("=== WPF Application Starting ===");
        Console.WriteLine($"Settings loaded from file: {settings.GetIsLoadedFromFile()}");
        Console.WriteLine($"Settings file: {Constant.pathSettings}");
        Console.WriteLine($"Championship: {settings.SelectedChampionship}");
        Console.WriteLine($"Language: {settings.SelectedLanguage}");

        // Apply language settings from shared settings file
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(settings.SelectedLanguage);
            Console.WriteLine($"UI Culture set to: {Thread.CurrentThread.CurrentUICulture.Name}");
        }
        catch (CultureNotFoundException ex)
        {
            Console.WriteLine($"Invalid culture '{settings.SelectedLanguage}': {ex.Message}");
            Console.WriteLine("Defaulting to English");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            settings.SelectedLanguage = "en";
        }

        // Check if settings have been loaded from file
        if (!settings.GetIsLoadedFromFile())
        {
            // Show settings window first
            var settingsWindow = new SettingsWindow();
            if (settingsWindow.ShowDialog() != true)
            {
                // User cancelled, exit application
                Shutdown();
                return;
            }
        }

        // Show main window
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
