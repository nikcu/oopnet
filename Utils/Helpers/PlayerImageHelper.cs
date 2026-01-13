namespace Utils.Helpers
{
    /// <summary>
    /// Helper class for player image generation and related utilities.
    /// Provides platform-agnostic methods that can be used by both WinForms and WPF.
    /// </summary>
    public static class PlayerImageHelper
    {
        /// <summary>
        /// Predefined colors for player placeholder images (as RGB tuples)
        /// </summary>
        public static readonly (int R, int G, int B)[] PlayerColors = new[]
        {
            (66, 133, 244),   // Blue
            (219, 68, 55),    // Red
            (244, 180, 0),    // Yellow
            (15, 157, 88),    // Green
            (171, 71, 188),   // Purple
            (0, 172, 193),    // Cyan
            (255, 112, 67),   // Orange
            (158, 157, 36)    // Olive
        };

        /// <summary>
        /// Default color when name is empty (Cornflower Blue)
        /// </summary>
        public static readonly (int R, int G, int B) DefaultColor = (100, 149, 237);

        /// <summary>
        /// Gets the initials from a player name
        /// </summary>
        /// <param name="name">Player's full name</param>
        /// <returns>1-2 character initials (uppercase)</returns>
        /// <example>
        /// GetInitials("Luka Modric") returns "LM"
        /// GetInitials("Ronaldo") returns "R"
        /// GetInitials("") returns "?"
        /// </example>
        public static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return "?";
            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();

            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }

        /// <summary>
        /// Gets a consistent color for a player based on their name
        /// </summary>
        /// <param name="name">Player's name</param>
        /// <returns>RGB color tuple</returns>
        public static (int R, int G, int B) GetPlayerColorRgb(string name)
        {
            if (string.IsNullOrEmpty(name))
                return DefaultColor;

            int hash = name.GetHashCode();
            return PlayerColors[Math.Abs(hash) % PlayerColors.Length];
        }

        /// <summary>
        /// Generates a unique identifier for a player
        /// </summary>
        /// <param name="name">Player's name</param>
        /// <param name="shirtNumber">Player's shirt number</param>
        /// <returns>Identifier in "name_number" format</returns>
        public static string GeneratePlayerIdentifier(string name, int shirtNumber)
        {
            // Normalize name: lowercase, replace spaces with underscores
            string normalizedName = name.ToLowerInvariant().Replace(" ", "_");
            return $"{normalizedName}_{shirtNumber}";
        }
    }
}
