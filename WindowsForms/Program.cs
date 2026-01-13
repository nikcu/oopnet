using DataLayer;
using System.Globalization;

// Both WindowsForms and WPF apps share the same settings file at the solution root
// See DataLayer.Constant.pathSettings

namespace WindowsForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize settings BEFORE creating any forms
            // This triggers the Singleton and loads settings.json if it exists
            // Settings file is shared between Windows Forms and WPF apps
            var settings = Settings.Instance;

            Console.WriteLine("=== Windows Forms Application Starting ===");
            Console.WriteLine($"Settings loaded from file: {settings.GetIsLoadedFromFile()}");
            Console.WriteLine($"Settings file: {Constant.pathSettings}");
            Console.WriteLine($"Championship: {settings.SelectedChampionship}");
            Console.WriteLine($"Language: {settings.SelectedLanguage}");

            // Set the UI culture based on loaded settings
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

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new SettingsForm());
        }
    }
}