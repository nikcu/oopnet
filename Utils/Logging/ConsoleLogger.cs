using Utils.Interfaces;

namespace Utils.Logging
{
    /// <summary>
    /// Console-based implementation of ILogger.
    /// Outputs log messages to the console with appropriate prefixes.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly string _context;

        public ConsoleLogger(string context = "")
        {
            _context = string.IsNullOrEmpty(context) ? "" : $"[{context}] ";
        }

        public void Info(string message)
        {
            Console.WriteLine($"{_context}[INFO] {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"{_context}[WARN] {message}");
        }

        public void Error(string message, Exception? ex = null)
        {
            Console.WriteLine($"{_context}[ERROR] {message}");
            if (ex != null)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
            }
        }

        public void Debug(string message)
        {
#if DEBUG
            Console.WriteLine($"{_context}[DEBUG] {message}");
#endif
        }
    }
}
