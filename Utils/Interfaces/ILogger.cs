namespace Utils.Interfaces
{
    /// <summary>
    /// Interface for logging operations.
    /// Provides abstraction over console output for better testability.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// Logs an error message with optional exception
        /// </summary>
        void Error(string message, Exception? ex = null);

        /// <summary>
        /// Logs a debug message (only in debug builds)
        /// </summary>
        void Debug(string message);
    }
}
