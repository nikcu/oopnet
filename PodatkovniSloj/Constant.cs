using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public static class Constant
    {
        // Data source types
        public const string DataSourceApi = "api";
        public const string DataSourceJson = "json";

        // Resource paths (relative to solution root)
        // From bin/Debug/net8.0-windows, go up 4 levels to reach solution root
        private static readonly string SolutionPath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));

        // Shared settings files (in solution root so both WinForms and WPF share them)
        public static readonly string pathSettings = Path.Combine(SolutionPath, "settings.json");
        public static readonly string pathDataConfig = Path.Combine(SolutionPath, "dataconfig.json");

        public static readonly string ResourcesPath = Path.Combine(SolutionPath, "Resources");
        public static readonly string ImagesPath = Path.Combine(ResourcesPath, "Images");

        // Player-related paths
        public static readonly string PlayerImagesPath = Path.Combine(ImagesPath, "Players");
        public static readonly string DefaultPlayerImagePath = Path.Combine(ImagesPath, "default_player.png");

        // Team-related paths
        public static readonly string FlagsPath = Path.Combine(ImagesPath, "Flags");
        public static readonly string EmblemsPath = Path.Combine(ImagesPath, "Emblems");

        // Helper method to generate player image path
        public static string GetPlayerImagePath(string championship, string teamFifaCode, string playerName, int shirtNumber, string extension)
        {
            string playerIdentifier = GeneratePlayerIdentifier(playerName, shirtNumber);
            string filename = $"{playerIdentifier}{extension}";
            return Path.Combine(PlayerImagesPath, championship, teamFifaCode, filename);
        }

        // Helper method to generate player identifier (used as dictionary key and filename base)
        public static string GeneratePlayerIdentifier(string playerName, int shirtNumber)
        {
            return $"{playerName}_{shirtNumber}"
                .Trim()
                .ToLower()
                .Replace(" ", "_")
                .Replace("-", "_");
        }
    }
}
